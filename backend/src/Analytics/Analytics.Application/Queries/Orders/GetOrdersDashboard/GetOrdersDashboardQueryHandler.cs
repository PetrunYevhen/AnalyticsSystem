using Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;
using Analytics.Domain.Entities.Order;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public class GetOrdersDashboardQueryHandler : IRequestHandler<GetOrdersDashboardQuery, OrdersDashboardDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetOrdersDashboardQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<OrdersDashboardDto> Handle(GetOrdersDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = @"
            SELECT COUNT(*) AS ""TotalOrders"",
           COUNT(CASE WHEN ""Status"" = 'Completed' THEN 1 END) AS ""SuccessfulOrders"", 
           COUNT(CASE WHEN ""Status"" = 'Pending' THEN 1 END) AS ""ProcessingOrders""
            FROM ""Analytics"".""Orders"";

            SELECT 
                o.""ExternalOrderId"" AS ""OrderNumber"", 
                c.""FullName"" AS ""CustomerName"", 
                o.""OrderDate"", 
                o.""Status"", 
                o.""TotalAmount""
            FROM ""Analytics"".""Orders"" o
            JOIN ""Analytics"".""Customers"" c ON o.""CustomerId"" = c.""Id""            
            ORDER BY o.""OrderDate"" DESC
            LIMIT 50;";

        using var multiQuery = await connection.QueryMultipleAsync(sql);
        var stats = await multiQuery.ReadSingleAsync<OrderStatsDto>();
        var listItems = (await multiQuery.ReadAsync<OrderListItemDto>()).ToList();
        
        return new OrdersDashboardDto(stats, listItems);

    }
}