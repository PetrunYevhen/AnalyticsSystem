using Analytics.Application.Auth;
using Analytics.Application.Queries.Users.GetUser.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Users.GetUser;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileInfoDto>
{
    private readonly ITenantContext _tenantContext;
    private readonly IUserContext _userContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetProfileQueryHandler(ITenantContext tenantContext, INpgsqlConnectionFactory connectionFactory, IUserContext userContext)
    {
        _tenantContext = tenantContext;
        _connectionFactory = connectionFactory;
        _userContext = userContext;
    }

    public async Task<ProfileInfoDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        var userId = _userContext.RequiredUserId();
        
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = """
                    SELECT 
                        "FullName",
                        "Email",
                        "Role"
                    FROM "Analytics"."Users"                   
                    WHERE "TenantId" = @TenantId
                    AND "Id" = @UserId
                    """;
        
        var query = await connection.QuerySingleOrDefaultAsync<ProfileInfoDto>(
            sql,
            new
            {
                TenantId = tenantId,  
                UserId = userId
            });
        
        if (query == null)
        {
            throw new Exception("Користувача не знайдено");
        }
        return query;

    }
}