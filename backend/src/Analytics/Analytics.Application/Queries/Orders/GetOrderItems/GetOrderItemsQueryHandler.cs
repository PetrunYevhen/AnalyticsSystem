using Analytics.Application.Auth;
using Analytics.Application.Queries.Orders.GetOrderItems.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Orders.GetOrderItems;

public class GetOrderItemsQueryHandler : IRequestHandler<GetOrderItemsQuery, Result<List<OrderItemDto>>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetOrderItemsQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<OrderItemDto>>> Handle(GetOrderItemsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                           SELECT
                               oi."Id" AS "ProductId",
                               oi."ProductName",
                               oi."Category" AS "ProductCategory",
                               oi."Quantity",
                               oi."UnitPrice",
                               oi."UnitCost",
                               oi."Currency"
                           FROM "Analytics"."OrderItem" oi
                           JOIN "Analytics"."Orders" o ON o."Id" = oi."OrderId"
                           WHERE o."TenantId" = @TenantId
                             AND oi."OrderId" = @OrderId;
                           """;

        var parameters = new
        {
            TenantId = tenantId,
            OrderId = request.OrderId
        };
        
        var query = await connection.QueryAsync<OrderItemDto>(sql, parameters);
        
        return Result.Ok(query.AsList());
    }
}