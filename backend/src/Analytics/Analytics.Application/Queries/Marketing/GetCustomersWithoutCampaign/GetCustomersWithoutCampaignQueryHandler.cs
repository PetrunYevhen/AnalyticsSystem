using Analytics.Application.Auth;
using Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign;

public class GetCustomersWithoutCampaignQueryHandler : IRequestHandler<GetCustomersWithoutCampaignQuery, Result<List<CampaignCustomerDto>>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetCustomersWithoutCampaignQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<CampaignCustomerDto>>> Handle(GetCustomersWithoutCampaignQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT "Id", "FullName", "Email", "PhoneNumber", "AcquisitionChannel"
                           FROM "Analytics"."Customers"
                           WHERE "TenantId" = @TenantId
                             AND "CampaignId" IS NULL 
                           ORDER BY "FullName" 
                           """;

        var parameters = new
        {
            TenantId = tenantId,
        };

        var query = await connection.QueryAsync<CampaignCustomerDto>(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        
        return Result.Ok(query.AsList());
    }
}