using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public class GetOrdersDashboardQueryHandler : IRequestHandler<GetOrdersDashboardQuery, OrdersDashboardDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;   
    private readonly ITenantContext _tenantContext;

    public GetOrdersDashboardQueryHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<OrdersDashboardDto> Handle(GetOrdersDashboardQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        
        if (!SortColumnMap.Columns.TryGetValue(request.SortBy, out var sortColumn))
            throw new ArgumentException($"Неможливо відсортувати {request.SortBy}");

        var direction = request.Direction == SortDirection.Asc ? "ASC" : "DESC";
        
        using var connection = _connectionFactory.CreateNewConnection();
        var search = request.Search is null 
            ? "" : """
                   AND (
                       o."ExternalOrderId" ILIKE @SearchPattern OR
                       c."FullName"        ILIKE @SearchPattern
                   )
                   """;
        
        string statsSql =
            $"""
                 SELECT COUNT(*) AS "TotalOrders",
                 COUNT(CASE WHEN "Status" = 'Completed' THEN 1 END) AS "SuccessfulOrders", 
                 COUNT(CASE WHEN "Status" = 'Pending' THEN 1 END) AS "ProcessingOrders"
                 FROM "Analytics"."Orders"
                 WHERE "TenantId" = @TenantId;
             """;
        
        string listSql = 
            $"""
            SELECT 
                COUNT(*) OVER() AS "TotalCount",
                o."Id" AS "OrderId",
                o."ExternalOrderId" AS "ExternalOrderId", 
                c."FullName" AS "CustomerName", 
                o."OrderDate", 
                o."Status", 
                o."TotalAmount"
            FROM "Analytics"."Orders" o
            JOIN "Analytics"."Customers" c ON o."CustomerId" = c."Id"  
            WHERE o."TenantId" = @TenantId
            {search}
            ORDER BY {sortColumn} {direction}
            LIMIT @PageSize OFFSET @Skip;
            """;

        var parameters = new
        {
            TenantId = tenantId,
            PageSize = request.PageSize,
            Skip = (request.Page - 1) * request.PageSize,
            SearchPattern = request.Search is null ? null : $"%{request.Search.Trim()}%",

        };
        
        using var multiQuery = await connection.QueryMultipleAsync(
            new CommandDefinition(statsSql + listSql, parameters, cancellationToken: cancellationToken));
        
        var stats = await multiQuery.ReadSingleAsync<OrderStatsDto>();
        var rows  = (await multiQuery.ReadAsync<OrderListRow>()).AsList();

        var total = rows.FirstOrDefault()?.TotalCount ?? 0;

        return new OrdersDashboardDto(
            stats,
            new PagedResult<OrderListItemDto>(
                request.Page,
                request.PageSize,
                total,
                rows.Select(r => r.ToDto()).ToList()
            )
        );
    }
    
    private class OrderListRow
    {
        public Guid OrderId { get; set; }
        public required string ExternalOrderId { get; set; }
        public required string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public required string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalCount { get; set; }
    
        public OrderListItemDto ToDto() => new(OrderId, ExternalOrderId, CustomerName, OrderDate, Status, TotalAmount);

    }
    
    
}