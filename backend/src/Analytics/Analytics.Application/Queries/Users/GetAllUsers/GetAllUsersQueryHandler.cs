using Analytics.Application.Auth;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Users.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<GetUsersDto>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetAllUsersQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
    }

    public async Task<List<GetUsersDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                               SELECT 
                                   "FullName",
                                   "Email",
                                   "Role",
                                   "CreatedAt"
                               FROM "Analytics"."Users"
                               WHERE "TenantId" = @TenantId
                           """;
        return (await connection.QueryAsync<GetUsersDto>(sql, new
        {
            TenantId = tenantId
        })).ToList();
    }
}