using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Cohort;
using Application;
using Dapper;

namespace Analytics.Infrastructure.StatsReader;

public class CohortStatsReader : ICohortStatsReader
{
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private static readonly TimeSpan ActiveWindow = TimeSpan.FromDays(90);

    public CohortStatsReader(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CohortStats> GetAsync(Guid tenantId, Period period, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                           WITH
                           at_start AS (
                               SELECT COUNT(*) AS cnt
                               FROM "Analytics"."Customers"
                               WHERE "TenantId"       = @TenantId
                                 AND "FirstOrderDate" < @StartUtc
                           ),
                           at_end AS (
                               SELECT COUNT(DISTINCT o."CustomerId") AS cnt
                               FROM "Analytics"."PaymentTransactions" pt
                               JOIN "Analytics"."Payments"  p ON p."Id" = pt."PaymentId"
                               JOIN "Analytics"."Orders"    o ON o."Id" = p."OrderId"
                               JOIN "Analytics"."Customers" c ON c."Id" = o."CustomerId"
                               WHERE pt."TenantId"      = @TenantId
                                 AND pt."Type"          = 'Payment'
                                 AND p."Status"         IN ('Paid', 'PartiallyPaid')
                                 AND c."FirstOrderDate" < @StartUtc
                                 AND pt."PaidAt"       >= @ActiveCutoffUtc
                                 AND pt."PaidAt"       <  @EndUtc
                           ),
                           avg_lifespan AS (
                               SELECT COALESCE(
                                   AVG(EXTRACT(EPOCH FROM ("LastOrderDate" - "FirstOrderDate")) / 86400.0), 0
                               ) AS days
                               FROM "Analytics"."Customers"
                               WHERE "TenantId"       = @TenantId
                                 AND "OrderCount"    >= 2
                                 AND "FirstOrderDate" < @EndUtc
                           )
                           SELECT
                               (SELECT cnt  FROM at_start)   AS "CustomersAtStart",
                               (SELECT cnt  FROM at_end)     AS "CustomersAtEnd",
                               (SELECT days FROM avg_lifespan) AS "AvgLifespanDays";
                           """;

        var parameters = new
        {
            TenantId = tenantId,
            StartUtc = period.StartUtc,
            EndUtc = period.EndUtc,
            ActiveCutoffUtc = period.EndUtc - ActiveWindow
        };
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        return await connection.QuerySingleAsync<CohortStats>(command);
    }
}