using Analytics.Application.Auth;
using Analytics.Application.Queries.Marketing.GetAllCampaigns.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Marketing.GetAllCampaigns;

public class GetAllCampaignsQueryHandler : IRequestHandler<GetAllCampaignsQuery, Result<List<CampaignDto>>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetAllCampaignsQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<List<CampaignDto>>> Handle(GetAllCampaignsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT 
                               c."Id",
                               c."Name",
                               c."Channel",
                               c."Status",
                               c."ActivePeriodStart" AS "ActivePeriodStart",
                               c."ActivePeriodEnd"   AS "ActivePeriodEnd",
                               c."BudgetAmount" AS "Budget",
                               c."BudgetCurrency" AS "Currency",
                               c."ActualSpendAmount" AS "ActualSpend",
                               COUNT(cu."Id") AS "CustomersCount"
                           FROM "Analytics"."Campaigns" c
                           LEFT JOIN "Analytics"."Customers" cu 
                               ON cu."CampaignId" = c."Id"
                              AND cu."TenantId" = c."TenantId"
                           WHERE c."TenantId" = @TenantId
                           GROUP BY c."Id", c."Name", c."Channel", c."Status",
                                    c."ActivePeriodStart", c."ActivePeriodEnd",
                                    c."BudgetAmount", c."BudgetCurrency"
                           ORDER BY c."CreatedAt" DESC
                           """;

        var parameters = new
        {
            TenantId = tenantId,
        };

        var query = await connection.QueryAsync<CampaignDto>(new CommandDefinition(sql, parameters));
        return Result.Ok(query.AsList());
    }
}