using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Acquisition;
using Application;
using Dapper;

namespace Analytics.Infrastructure.StatsReader;

public class AcquisitionStatsReader : IAcquisitionStatsReader
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public AcquisitionStatsReader(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    public async Task<AcquisitionStats> GetAsync(Guid tenantId, Period period, CancellationToken cancellationToken = default)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
            WITH
            new_customers AS (
                SELECT COUNT(*) AS cnt
            FROM "Analytics"."Customers"
            WHERE "TenantId" = @TenantId
            AND "FirstOrderDate" >= @StartUtc
            AND "FirstOrderDate" <  @EndUtc
            ),
            marketing_spend AS (
            SELECT COALESCE(SUM("Amount"), 0) AS amount
            FROM "Analytics"."MarketingExpenses"
            WHERE "TenantId"    = @TenantId
              AND "ExpenseDate" >= @StartDate
              AND "ExpenseDate" <  @EndDate 
            ),
            repeat_customers AS (
            SELECT
                COUNT(*) FILTER (WHERE "OrderCount" > 1) AS repeat_count,
                COUNT(*) AS total_count
            FROM "Analytics"."Customers"
            WHERE "TenantId" = @TenantId
                AND "FirstOrderDate" >= @StartUtc 
                AND "FirstOrderDate" <  @EndUtc 
            ),
            campaign_spend AS (
            SELECT COALESCE(SUM("ActualSpendAmount"), 0) AS amount
            FROM "Analytics"."Campaigns"
            WHERE "TenantId"          = @TenantId
              AND "ActivePeriodStart" <  @EndDate
              AND "ActivePeriodEnd"   >= @StartDate
             )
            SELECT
                (SELECT cnt FROM new_customers) AS "NewCustomers",
            (SELECT amount FROM marketing_spend) + (SELECT amount FROM campaign_spend) AS "MarketingSpend",
            (SELECT repeat_count FROM repeat_customers) AS "RepeatCustomers",
            (SELECT total_count  FROM repeat_customers) AS "TotalCustomers";
    """;

        var parameters = new
        {
                TenantId = tenantId,
                StartUtc = period.StartUtc,
                EndUtc = period.EndUtc,
                StartDate = DateOnly.FromDateTime(period.StartUtc),
                EndDate = DateOnly.FromDateTime(period.EndUtc)
        };
        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        
        var result = await connection.QuerySingleOrDefaultAsync<AcquisitionStats>(command);
        return result;
    }
}
