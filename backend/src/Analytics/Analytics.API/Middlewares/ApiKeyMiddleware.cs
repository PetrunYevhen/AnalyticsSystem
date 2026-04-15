using System.Security.Claims;
using Analytics.Application.Auth.ApiKey;
using Analytics.Domain.RepositoryContracts;

namespace Analytics.API.Middlewares;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITenantRepository _tenantRepository;
    
    public ApiKeyMiddleware(RequestDelegate next, ITenantRepository tenantRepository)
    {
        _next = next;
        _tenantRepository = tenantRepository;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }
        
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var rawKey))
        {
            await _next(context);
            return;
        }
        
        var plainKey = rawKey.ToString().Trim();
        if (plainKey.Length < 15)
        {
            context.Response.StatusCode = 401;
            return;
        }
        
        var prefix = plainKey[..15];
        var tenant = await _tenantRepository.GetByApiPrefixAsync(prefix);
        
        if (tenant is null || !apiKeyService.Verify(plainKey, tenant.ApiKeyHash))
        {
            context.Response.StatusCode = 401;
            return;
        }
        var claims = new[]
        {
            new Claim("tenantId", tenant.Id.ToString()),
        };
        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.User = new ClaimsPrincipal(identity);

        await _next(context);

    }
}