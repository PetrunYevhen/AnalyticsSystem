using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Revenue;
using Application;
using Dapper;

namespace Analytics.Infrastructure.StatsReader;

public class RevenueStatsReader : IRevenueStatsReader
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public RevenueStatsReader(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RevenueStats> GetAsync(Guid tenantId, Period period, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT
                               COALESCE(SUM(pt."Amount"), 0)  AS "TotalRevenue",
                               COUNT(DISTINCT p."OrderId") AS "OrdersCount",
                               COUNT(DISTINCT o."CustomerId") AS "UniqueCustomers"
                           FROM "Analytics"."PaymentTransactions" pt
                           JOIN "Analytics"."Payments" p ON p."Id" = pt."PaymentId"
                           JOIN "Analytics"."Orders"   o ON o."Id" = p."OrderId"
                           WHERE pt."TenantId" = @TenantId
                             AND pt."Type" = 'Payment'
                             AND p."Status" IN ('Paid', 'PartiallyPaid')
                             AND pt."PaidAt"  >= @StartUtc
                             AND pt."PaidAt" <  @EndUtc;
                           """;

        var cmd = new CommandDefinition(sql, new
        {
            TenantId = tenantId,
            Currency = "UAH",                
            Statuses = new[] { "Paid", "PartiallyPaid" },
            period.StartUtc,
            period.EndUtc
        }, cancellationToken: ct);

        return await connection.QuerySingleAsync<RevenueStats>(cmd);
    }
}