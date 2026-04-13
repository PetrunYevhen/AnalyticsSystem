using Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;
using Analytics.Application.Queries.Transactions.GetTransactiosDashboard.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Transactions.GetTransactiosDashboard;

public class GetTransactionsDashboardQueryHandler : IRequestHandler<GetTransactionsDashboardQuery, TransactionsDashboardDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetTransactionsDashboardQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TransactionsDashboardDto> Handle(GetTransactionsDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                    SELECT 
                    COALESCE(SUM(CASE WHEN "Status" = 'Completed' THEN "TotalAmount" ELSE 0 END), 0) AS "AvailableBalance",
                    COALESCE(SUM(CASE WHEN "Status" = 'Completed' AND DATE("OrderDate") = CURRENT_DATE - INTERVAL '1 day' THEN "TotalAmount" ELSE 0 END), 0) AS "YesterdayRevenue",
                    COALESCE(SUM(CASE WHEN "Status" = 'Refunded' THEN "TotalAmount" ELSE 0 END), 0) AS "RefundedAmount"
                    FROM "Analytics"."Orders";
                    
                    SELECT 
                    o."ExternalOrderId" AS "TransactionId", 
                    c."FullName" AS "CustomerName",
                    'Card' AS "Method",
                    o."OrderDate" AS "Date", 
                        o."Status" AS "Status", 
                        o."TotalAmount" AS "Amount"
                    FROM "Analytics"."Orders" o
                    JOIN "Analytics"."Customers" c ON o."CustomerId" = c."Id"
                    ORDER BY o."OrderDate" DESC
                    LIMIT 50;
                    """;

        using var multiQuery = await connection.QueryMultipleAsync(sql);
        var stats = await multiQuery.ReadSingleOrDefaultAsync<TransactionStatsDto>();
        var transactions = (await multiQuery.ReadAsync<TransactionItemDto>()).ToList(); 
        return new TransactionsDashboardDto(
            stats,
            transactions
        );
    }
}