using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Tenants.GetTenant;

public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetTenantQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        
        const string sql = @"
                            SELECT 
                                ""Id"", 
                                ""Name"", 
                                ""Email"", 
                                ""CreatedAt""
                            FROM ""Analytics"".""Tenants"" 
                            WHERE ""Id"" = @Id";        
        var tenant = await connection.QueryFirstOrDefaultAsync<TenantDto>(sql, new { Id = request.TenantId });
        return tenant;
    }
}