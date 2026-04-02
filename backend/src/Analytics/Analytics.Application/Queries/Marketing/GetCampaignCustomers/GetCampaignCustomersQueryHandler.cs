using Analytics.Application.Auth;
using Analytics.Application.Queries.Marketing.GetCampaignCustomers.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Marketing.GetCampaignCustomers;

public class GetCampaignCustomersQueryHandler : IRequestHandler<GetCampaignCustomersQuery, Result<List<CampaignCustomerDto>>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetCampaignCustomersQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<CampaignCustomerDto>>> Handle(GetCampaignCustomersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                            SELECT "Id", "FullName", "Email", "PhoneNumber", "AcquisitionChannel"
                            FROM "Analytics"."Customers"
                            WHERE "TenantId" = @TenantId
                              AND "CampaignId" = @CampaignId
                            
                            ORDER BY "FullName"
                            """;

        var parameters = new
        {
            TenantId = tenantId,
            CampaignId = request.CampaignId
        };

        var query = await connection.QueryAsync<CampaignCustomerDto>
            (new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        return Result.Ok(query.AsList());
    }
}