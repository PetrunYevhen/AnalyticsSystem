using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Customers.GetAllCustomers.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Customers.GetAllCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly INpgsqlConnectionFactory _connectionFactory ;
    private readonly ITenantContext _tenantContext ;
    
    
    public GetAllCustomersQueryHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken ct)
    {
        var tenantId = _tenantContext.RequiredTenantId();
    
        if(!SortColumnMap.Columns.TryGetValue(request.SortBy, out var sortColumn))
            throw new ArgumentException($"Неможливо відсортувати {request.SortBy}");
        
        var direction = request.Direction == SortDirection.Asc ? "ASC" : "DESC";
        
        using var connection = _connectionFactory.CreateNewConnection();
        
        var search = request.Search is null
            ? ""
            : """
                AND (
              "FullName"     ILIKE @SearchPattern OR
              "Email"        ILIKE @SearchPattern OR
              "PhoneNumber"  ILIKE @SearchPattern)                                   
              """;
        var noCampaign = request.WithoutCampaign == true
            ? """AND "CampaignId" IS NULL"""
            : "";
        
        string sql = $"""
                       SELECT "Id", "FullName", "Email", 
                              TRIM("Status") AS "Status",
                              "LastOrderDate", "AcquisitionChannel", "TotalRevenue",
                       COUNT(*) OVER() AS "TotalCount"
                       FROM "Analytics"."Customers"
                       WHERE "TenantId" = @TenantId  
                       {search}
                       {noCampaign}
                         ORDER BY {sortColumn} {direction} NULLS LAST, "Id" ASC
                       LIMIT @PageSize OFFSET @Skip
                       """;
        
        var parameters = new
        {
            TenantId = tenantId,
            PageSize = request.PageSize,
            Skip = (request.Page - 1) * request.PageSize,
            SearchPattern   = request.Search is null ? null : $"%{request.Search.Trim()}%",
        };
        
        var rows = (await connection.QueryAsync<CustomerRow>(sql, parameters)).AsList();
        
        var total = rows.FirstOrDefault()?.TotalCount ?? 0;

        return new PagedResult<CustomerDto>(
            request.Page,  
            request.PageSize,
            total,
            rows.Select(r => r.ToDto()).ToList()
            );
    
    }
    
    private class CustomerRow
    {
        public Guid Id  { get; init; }
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public required string Status  { get; init; }
        public DateTime LastOrderDate  { get; init; }
        public required string AcquisitionChannel { get; init; }
        public decimal TotalRevenue { get; init; }
        public int TotalCount { get; init; }

        public CustomerDto ToDto() => new(Id, FullName, Email, Status, LastOrderDate, AcquisitionChannel, TotalRevenue);
    
    }
    
}