using Analytics.Application.Auth;
using Analytics.Application.Common.Csv;
using Analytics.Application.Queries.Marketing.ExportMarketingExpenses.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Marketing.ExportMarketingExpenses;

public class ExportMarketingExpensesQueryHandler : IRequestHandler<ExportMarketingExpensesQuery, byte[]>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ExportMarketingExpensesQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async  Task<byte[]> Handle(ExportMarketingExpensesQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT
                               me."Id"           AS "ExpenseId",
                               me."AdSource"     AS "AdSource",
                               me."Amount"       AS "Amount",
                               me."ExpenseDate"  AS "ExpenseDate",
                               me."Impressions"  AS "Impressions",
                               me."Clicks"       AS "Clicks",
                               me."Leads"        AS "Leads",
                               c."Name"          AS "CampaignName"
                               me."Currency" AS "Currency",
                           FROM "Analytics"."MarketingExpenses" me
                           LEFT JOIN "Analytics"."Campaigns" c ON c."Id" = me."CampaignId"
                           WHERE me."TenantId" = @TenantId
                             AND me."ExpenseDate" >= @StartUtc
                             AND me."ExpenseDate" <  @EndUtc
                           ORDER BY me."ExpenseDate" DESC
                           """;   
        
        var rows = await connection.QueryAsync<ExportMarketingExpenseDto>(sql, new
        {
            TenantId = tenantId,
            request.Period.StartUtc,
            request.Period.EndUtc
        });

        return await ExportToCsv.ToCsv(rows);
    }
}