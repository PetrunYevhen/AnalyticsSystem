using Analytics.Application.Auth;
using Analytics.Application.Common.Csv;
using Analytics.Application.Queries.Orders.ExportOrders.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Orders.ExportOrders;

public class ExportOrdersQueryHandler : IRequestHandler<ExportOrdersQuery, byte[]>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ExportOrdersQueryHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<byte[]> Handle(ExportOrdersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                           SELECT
                               o."Id"               AS "OrderId",
                               o."ExternalOrderId"  AS "ExternalOrderId",
                               c."FullName"         AS "CustomerName",
                               c."Email"            AS "CustomerEmail",
                               o."OrderDate"        AS "OrderDate",
                               o."Status"           AS "Status",
                               o."TotalAmount"      AS "TotalAmount"
                           FROM "Analytics"."Orders" o
                           JOIN "Analytics"."Customers" c ON c."Id" = o."CustomerId"
                           WHERE o."TenantId" = @TenantId
                             AND o."OrderDate" >= @StartUtc
                             AND o."OrderDate" <  @EndUtc
                           ORDER BY o."OrderDate" DESC
                           """;

        var rows = await connection.QueryAsync<ExportOrderDto>(sql, new
        {
            TenantId = tenantId,
            request.Period.StartUtc,
            request.Period.EndUtc
        });

        return await ExportToCsv.ToCsv(rows);
    }
}