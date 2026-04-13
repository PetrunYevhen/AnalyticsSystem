using Analytics.Application.Queries.MarketingExpenses.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.MarketingExpenses;

public class GetMarketingExpensesDashdoardQueryHandler : IRequestHandler<GetMarketingExpensesDashboardQuery, MarketingExpensesDashdoardDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetMarketingExpensesDashdoardQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<MarketingExpensesDashdoardDto> Handle(GetMarketingExpensesDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        const string sql = """
                            WITH ExpenseTotals AS (
                                SELECT 
                                    COALESCE(SUM("Amount"), 0) AS TotalAdSpend,
                                    COALESCE(SUM("Clicks"), 0) AS TotalClicks,
                                    COALESCE(SUM("Leads"), 0) AS TotalLeads
                                FROM "Analytics"."MarketingExpenses"
                            ),
                            CustomerTotals AS (
                            SELECT COUNT(*) AS NewCustomers 
                            FROM "Analytics"."Customers"
                            WHERE "Status" = '1'
                            ),
                            RevenueTotals AS(
                            SELECT COALESCE(SUM(o."TotalAmount"), 0) AS TotalRevenue
                            FROM "Analytics"."Orders" o
                            JOIN "Analytics"."Customers" c ON o."CustomerId" = c."Id"
                            WHERE o."Status" = '3' 
                              AND c."AcquisitionChannel" IS NOT NULL
                            )
                            SELECT 
                                e.TotalAdSpend AS "TotalAdSpend",
                                -- CAC
                                (e.TotalAdSpend / NULLIF(c.NewCustomers, 0)) AS "Cac",
                                -- ROMI
                                ((r.TotalRevenue - e.TotalAdSpend) / NULLIF(e.TotalAdSpend, 0)) * 100 AS "Romi",
                                -- Конверсія
                                (e.TotalLeads::decimal / NULLIF(e.TotalClicks, 0)) * 100 AS "ConversionRate"
                            FROM ExpenseTotals e, CustomerTotals c, RevenueTotals r;

                            WITH ChannelRevenue AS (
                                SELECT 
                                    c."AcquisitionChannel",
                                    COALESCE(SUM(o."TotalAmount"), 0) AS Revenue
                                FROM "Analytics"."Customers" c
                                JOIN "Analytics"."Orders" o ON c."Id" = o."CustomerId"
                                WHERE o."Status" = '3' 
                                GROUP BY c."AcquisitionChannel"
                            )
                            SELECT 
                                e."AdSource" AS "Source",
                                COALESCE(SUM(e."Clicks"), 0) AS "Clicks",
                                COALESCE(SUM(e."Amount"), 0) AS "Spend",
                                COALESCE(cr.Revenue, 0) AS "Revenue",
                                -- ROAS
                                (COALESCE(cr.Revenue, 0) / NULLIF(SUM(e."Amount"), 0)) AS "Roas"
                            FROM "Analytics"."MarketingExpenses" e
                            LEFT JOIN ChannelRevenue cr ON e."AdSource" = cr."AcquisitionChannel"
                            GROUP BY e."AdSource", cr.Revenue
                            ORDER BY "Spend" DESC;
                            """;

        var multiQuery = await connection.QueryMultipleAsync(sql);
        var stats = await multiQuery.ReadFirstOrDefaultAsync<MarketingStatsDto>();
        var trafficList = (await multiQuery.ReadAsync<TrafficSourceItemDto>()).ToList();
        
        return new MarketingExpensesDashdoardDto(stats, trafficList);

    }
}