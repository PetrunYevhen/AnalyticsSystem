using Analytics.Application.Auth;
using Analytics.Application.Common.Csv;
using Analytics.Application.Queries.Customers.ExportCustomers.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Customers.ExportCustomers;

public class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, byte[]>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ExportCustomersQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<byte[]> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;
        var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = """
                           SELECT
                               c."Id"                  AS "CustomerId",
                               c."FullName"            AS "FullName",
                               c."Email"               AS "Email",
                               c."PhoneNumber"         AS "PhoneNumber",
                               c."RegistrationDate"    AS "RegistrationDate",
                               c."FirstOrderDate"      AS "FirstOrderDate",
                               c."LastOrderDate"       AS "LastOrderDate",
                               c."OrderCount"          AS "OrderCount",
                               TRIM(c."Status")        AS "Status",
                               c."AcquisitionChannel"  AS "AcquisitionChannel",
                               c."TotalRevenue"       AS "TotalRevenue"
                           FROM "Analytics"."Customers" c
                           WHERE c."TenantId" = @TenantId
                             AND c."FirstOrderDate" >= @StartUtc
                             AND c."FirstOrderDate" <  @EndUtc
                           ORDER BY c."LastOrderDate" DESC
                           """;
        
        var rows = await connection.QueryAsync<ExportCustomerDto>(sql, new
        {
            TenantId = tenantId,
            request.Period.StartUtc,
            request.Period.EndUtc
        });

        return await ExportToCsv.ToCsv(rows, cancellationToken);
    }
    
    
}