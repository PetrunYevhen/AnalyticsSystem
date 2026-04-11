using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Cost;
using Application;
using Dapper;

namespace Analytics.Infrastructure.StatsReader;

public class CostStatsReader : ICostStatsReader
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public CostStatsReader(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CostStats> GetAsync(Guid tenantId, Period period, CancellationToken ct = default)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT COALESCE(SUM(oi."UnitCost" * oi."Quantity"), 0) AS "TotalCogs"
                           FROM "Analytics"."PaymentTransactions" pt
                           JOIN "Analytics"."Payments" p ON p."Id" = pt."PaymentId"
                           JOIN "Analytics"."Orders" o ON o."Id" = p."OrderId"
                           JOIN "Analytics"."OrderItem" oi ON oi."OrderId" = o."Id"
                           WHERE pt."TenantId" = @TenantId
                             AND pt."Type" = 'Payment'
                             AND p."Status" IN ('Paid', 'PartiallyPaid')
                             AND pt."PaidAt"  >= @StartUtc
                             AND pt."PaidAt" < @EndUtc;
                           """;

        var command = new CommandDefinition(sql, new
        {
            TenantId = tenantId,
            period.StartUtc,
            period.EndUtc
        }, cancellationToken: ct);

        return await connection.QuerySingleAsync<CostStats>(command);
    }
}