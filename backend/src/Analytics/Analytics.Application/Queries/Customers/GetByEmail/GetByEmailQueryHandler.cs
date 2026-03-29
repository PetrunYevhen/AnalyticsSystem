using Analytics.Application.Auth;
using Analytics.Application.Queries.Customers.GetByEmail.Dtos;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Queries.Customers.GetByEmail;

public class GetByEmailQueryHandler : IRequestHandler<GetByEmailQuery, Result<CustomerDto>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetByEmailQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<Result<CustomerDto>> Handle(GetByEmailQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                            SELECT "Id",
                           "FullName",
                           "Email",
                           "PhoneNumber"
                            FROM "Analytics"."Customers"
                            WHERE "TenantId" = @TenantId
                            AND "Email" = @Email
                           """;

        var parameters = new
        {
            TenantId = tenantId,
            Email = request.Email
        };

        var query = await connection.QueryFirstOrDefaultAsync<CustomerDto>(sql, parameters);
        
        if (query is null)
            return Result.Fail("Клієнта не знайдено.");

        return Result.Ok(query);
    }
}