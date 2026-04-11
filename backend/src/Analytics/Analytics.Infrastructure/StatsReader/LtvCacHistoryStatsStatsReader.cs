using Analytics.Application.Common;
using Analytics.Application.Queries.Dashboard.Enums;
using Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;
using Analytics.Application.Metrics.Readers.LtvCac;
using Application;
using Dapper;

namespace Analytics.Infrastructure.StatsReader;

public class LtvCacHistoryStatsStatsReader : ILtvCacHistoryStatsReader
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public LtvCacHistoryStatsStatsReader(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<LtvCacDataPoint>> GetAsync(
        Guid tenantId,
        Period period,
        Granularity granularity,
        decimal lifespanMonths,
        decimal grossMargin,
        CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        var truncate = granularity switch
        {
            Granularity.Daily   => "day",
            Granularity.Weekly  => "week",
            Granularity.Monthly => "month",
            _                   => "month"
        };

        var sql = $"""
            WITH periods AS (
                SELECT generate_series(
                    date_trunc('{truncate}', @StartUtc::timestamptz),
                    date_trunc('{truncate}', @EndUtc::timestamptz),
                    '1 {truncate}'::interval
                ) AS period_start
            ),
            revenue_by_period AS (
                SELECT
                    date_trunc('{truncate}', pt."PaidAt") AS period_start,
                    COALESCE(SUM(pt."Amount"), 0)         AS total_revenue,
                    COUNT(DISTINCT o."CustomerId")        AS unique_customers,
                    COUNT(DISTINCT p."OrderId")           AS orders_count
                FROM "Analytics"."PaymentTransactions" pt
                JOIN "Analytics"."Payments" p ON p."Id" = pt."PaymentId"
                JOIN "Analytics"."Orders"   o ON o."Id" = p."OrderId"
                WHERE pt."TenantId" = @TenantId
                  AND pt."Type"     = 'Payment'
                  AND p."Status"    IN ('Paid', 'PartiallyPaid')
                  AND pt."PaidAt"  >= @StartUtc
                  AND pt."PaidAt"  <  @EndUtc
                GROUP BY date_trunc('{truncate}', pt."PaidAt")
            ),
            spend_by_period AS (
                SELECT
                    date_trunc('{truncate}', me."ExpenseDate"::timestamptz) AS period_start,
                    COALESCE(SUM(me."Amount"), 0) AS total_spend
                FROM "Analytics"."MarketingExpenses" me
                WHERE me."TenantId"    = @TenantId
                  AND me."ExpenseDate" >= @StartUtc::date
                  AND me."ExpenseDate" <  @EndUtc::date
                GROUP BY date_trunc('{truncate}', me."ExpenseDate"::timestamptz)
            ),
            new_customers_by_period AS (
                SELECT
                    date_trunc('{truncate}', c."FirstOrderDate") AS period_start,
                    COUNT(*) AS new_customers
                FROM "Analytics"."Customers" c
                WHERE c."TenantId"       = @TenantId
                  AND c."FirstOrderDate" >= @StartUtc
                  AND c."FirstOrderDate" <  @EndUtc
                GROUP BY date_trunc('{truncate}', c."FirstOrderDate")
            )
            SELECT
                p.period_start                                          AS "PeriodStart",
                COALESCE(r.total_revenue, 0)                           AS "TotalRevenue",
                COALESCE(r.unique_customers, 0)                        AS "UniqueCustomers",
                COALESCE(r.orders_count, 0)                            AS "OrdersCount",
                COALESCE(s.total_spend, 0)                             AS "TotalSpend",
                COALESCE(nc.new_customers, 0)                          AS "NewCustomers"
            FROM periods p
            LEFT JOIN revenue_by_period        r  ON r.period_start  = p.period_start
            LEFT JOIN spend_by_period          s  ON s.period_start  = p.period_start
            LEFT JOIN new_customers_by_period  nc ON nc.period_start = p.period_start
            ORDER BY p.period_start
            """;

        var rows = await connection.QueryAsync<LtvCacRow>(sql, new
        {
            TenantId = tenantId,
            period.StartUtc,
            period.EndUtc
        });

        return rows.Select(r => new LtvCacDataPoint
        {
            Date = FormatDate(r.PeriodStart, granularity),
            Ltv = CalculateLtv(r, lifespanMonths, grossMargin),
            Cac = r.NewCustomers == 0 ? 0m : r.TotalSpend / r.NewCustomers,
        }).ToList();
    }

    private static string FormatDate(DateTime date, Granularity granularity) => granularity switch
    {
        Granularity.Daily   => date.ToString("dd MMM"),
        Granularity.Weekly  => $"W{System.Globalization.ISOWeek.GetWeekOfYear(date)} {date.Year}",
        Granularity.Monthly => date.ToString("MMM yyyy"),
        _                   => date.ToString("MMM yyyy")
    };

    private static decimal CalculateLtv(LtvCacRow r, decimal lifespanMonths, decimal grossMargin)
    {
        if (r.UniqueCustomers == 0 || lifespanMonths <= 0) return 0m;
        var arpuPeriod = r.TotalRevenue / r.UniqueCustomers;
        return arpuPeriod * grossMargin * lifespanMonths;
    }

    private class LtvCacRow
    {
        public DateTime PeriodStart   { get; init; }
        public decimal TotalRevenue  { get; init; }
        public int UniqueCustomers { get; init; }
        public int OrdersCount   { get; init; }
        public decimal TotalSpend    { get; init; }
        public int NewCustomers  { get; init; }
    }
}