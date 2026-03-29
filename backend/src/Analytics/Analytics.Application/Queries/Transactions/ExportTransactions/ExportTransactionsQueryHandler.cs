using Analytics.Application.Auth;
using Analytics.Application.Common.Csv;
using Analytics.Application.Queries.Transactions.ExportTransactions.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Transactions.ExportTransactions;

public class ExportTransactionsQueryHandler : IRequestHandler<ExportTransactionsQuery, byte[]>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ExportTransactionsQueryHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<byte[]> Handle(ExportTransactionsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT
                               pt."Id"          AS "TransactionId",
                               pt."PaymentId"   AS "PaymentId",
                               o."ExternalOrderId" AS "ExternalOrderId",
                               c."FullName"     AS "CustomerName",
                               pt."Amount"      AS "Amount",
                               pt."Method"      AS "Method",
                               pt."Type"        AS "Type",
                               pt."Note"        AS "Note",
                               pt."PaidAt"      AS "PaidAt"
                           FROM "Analytics"."PaymentTransactions" pt
                           JOIN "Analytics"."Payments" p ON p."Id" = pt."PaymentId"
                           JOIN "Analytics"."Orders"   o ON o."Id" = p."OrderId"
                           JOIN "Analytics"."Customers" c ON c."Id" = o."CustomerId"
                           WHERE o."TenantId" = @TenantId
                             AND pt."PaidAt" >= @StartUtc
                             AND pt."PaidAt" <  @EndUtc
                           ORDER BY pt."PaidAt" DESC
                           """;

        var rows = await connection.QueryAsync<ExportTransactionsDto>(sql, new
        {
            TenantId = tenantId,
            request.Period.StartUtc,
            request.Period.EndUtc
        });

        return await ExportToCsv.ToCsv(rows);
    }
}