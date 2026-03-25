using Analytics.Application.Auth;
using Analytics.Application.Queries.Tenants.GetTenant.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Tenants.GetTenant;

public class GetTenantQueryHandler 
    : IRequestHandler<GetTenantQuery, Result<TenantInfoDto>>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private readonly ITenantContext _tenantContext;

    public GetTenantQueryHandler(
        INpgsqlConnectionFactory connectionFactory,
        ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<TenantInfoDto>> Handle(
        GetTenantQuery request, 
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        
        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                           SELECT 
                               "Id",
                               "CompanyName",
                               "ApiKeyPrefix",
                               "CreatedAt"
                           FROM "Analytics"."Tenants" 
                           WHERE "Id" = @Id
                           """;

        var parameters = new
        {
            Id = tenantId,
        };
        
        var query = await connection.QueryFirstOrDefaultAsync<TenantInfoDto>(
            new CommandDefinition(
                sql, 
                parameters,
                cancellationToken: cancellationToken));
        
        return Result.Ok(query);
    }
}