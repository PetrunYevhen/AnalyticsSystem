using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Queries.Dashboard.Dtos;
using Analytics.Application.Queries.Dashboard.Enums;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Dashboard;

public class GetDashboardQueryHandler 
    : IRequestHandler<GetDashboardQuery, Result<DashboardStatsResponse>>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private readonly ITenantContext _tenantContext;

    public GetDashboardQueryHandler(
        INpgsqlConnectionFactory connectionFactory,
        ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<DashboardStatsResponse>> Handle(
    GetDashboardQuery request,
    CancellationToken cancellationToken)
{
    var tenantId = _tenantContext.RequiredTenantId();

    var periodResult = Period.ResolvePeriod(request.FromDate, request.ToDate);
    if (periodResult.IsFailed)
        return Result.Fail(periodResult.Errors);

    var period = periodResult.Value;
    var previousStart = period.StartUtc - period.Duration;
    var previousEnd = period.StartUtc;

    var trunc = request.Granularity switch
    {
        Granularity.Daily   => "DATE_TRUNC('day', pt.\"PaidAt\")",
        Granularity.Weekly  => "DATE_TRUNC('week', pt.\"PaidAt\")",
        Granularity.Monthly => "DATE_TRUNC('month', pt.\"PaidAt\")",
        _ => throw new ArgumentOutOfRangeException(nameof(request.Granularity))
    };

    var sql = $"""
        WITH CurrentPeriod AS (
            SELECT
                COALESCE(SUM("TotalAmount"), 0) AS Revenue,
                COUNT(*) AS OrdersCount
            FROM "Analytics"."Orders"
            WHERE "TenantId" = @TenantId
              AND "Status" = 'Completed'
              AND "OrderDate" >= @Start AND "OrderDate" < @End
        ),
        PreviousPeriod AS (
            SELECT
                COALESCE(SUM("TotalAmount"), 0) AS Revenue,
                COUNT(*) AS OrdersCount
            FROM "Analytics"."Orders"
            WHERE "TenantId" = @TenantId
              AND "Status" = 'Completed'
              AND "OrderDate" >= @PreviousStart AND "OrderDate" < @PreviousEnd
        ),
        CurrentCustomers AS (
            SELECT COUNT(*) AS Count
            FROM "Analytics"."Customers"
            WHERE "TenantId" = @TenantId
              AND "RegistrationDate" >= @Start AND "RegistrationDate" < @End
        ),
        PreviousCustomers AS (
            SELECT COUNT(*) AS Count
            FROM "Analytics"."Customers"
            WHERE "TenantId" = @TenantId
              AND "RegistrationDate" >= @PreviousStart AND "RegistrationDate" < @PreviousEnd
        )
        SELECT
            cp.Revenue AS "RevenueValue",
            CASE WHEN pp.Revenue > 0
                 THEN ((cp.Revenue - pp.Revenue) / pp.Revenue * 100)::double precision
                 ELSE 0 END AS "RevenueChange",
            cc.Count::decimal AS "CustomersValue",
            CASE WHEN pc.Count > 0
                 THEN ((cc.Count - pc.Count)::double precision / pc.Count * 100)
                 ELSE 0 END AS "CustomersChange",
            cp.OrdersCount::decimal AS "ActivityValue",
            CASE WHEN pp.OrdersCount > 0
                 THEN ((cp.OrdersCount - pp.OrdersCount)::double precision / pp.OrdersCount * 100)
                 ELSE 0 END AS "ActivityChange"
        FROM CurrentPeriod cp, PreviousPeriod pp, CurrentCustomers cc, PreviousCustomers pc;

        SELECT
            o."Id"          AS "Id",
            c."FullName"    AS "CustomerName",
            c."Email"       AS "CustomerEmail",
            o."Status"      AS "Status",
            o."OrderDate"   AS "TransactionDate",
            o."TotalAmount" AS "Amount"
        FROM "Analytics"."Orders" o
        JOIN "Analytics"."Customers" c ON o."CustomerId" = c."Id"
        WHERE o."TenantId" = @TenantId
        ORDER BY o."OrderDate" DESC
        LIMIT 10;

        SELECT
            {trunc} AS "Date",
            SUM(pt."Amount") AS "Revenue"
        FROM "Analytics"."PaymentTransactions" pt
        WHERE pt."TenantId" = @TenantId
          AND pt."Type" = 'Payment'
          AND pt."PaidAt" >= @Start AND pt."PaidAt" < @End
        GROUP BY 1
        ORDER BY 1;
        """;

    var parameters = new
    {
        TenantId = tenantId,
        Start = period.StartUtc,
        End = period.EndUtc,
        PreviousStart = previousStart,
        PreviousEnd = previousEnd
    };

    using var connection = _connectionFactory.CreateNewConnection();
    using var multiQuery = await connection.QueryMultipleAsync(
        new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

    var kpis = await multiQuery.ReadFirstAsync<DashboardKpisRaw>();
    var recentTransactions = (await multiQuery.ReadAsync<RecentTransactionsDto>()).ToList();
    var revenueChart = (await multiQuery.ReadAsync<RevenueDataPointDto>()).ToList();

    return Result.Ok(new DashboardStatsResponse
    {
        TotalRevenue = new StatItemDto(kpis.RevenueValue, kpis.RevenueChange),
        NewCustomers = new StatItemDto(kpis.CustomersValue, kpis.CustomersChange),
        Activity = new StatItemDto(kpis.ActivityValue, kpis.ActivityChange),
        RecentTransactions = recentTransactions,
        RevenueChart = revenueChart
    });
}

    private record DashboardKpisRaw(
        decimal RevenueValue,
        double RevenueChange,
        decimal CustomersValue,
        double CustomersChange,
        decimal ActivityValue,
        double ActivityChange
    );
}