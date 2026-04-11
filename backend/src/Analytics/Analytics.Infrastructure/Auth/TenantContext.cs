using Analytics.Application.Auth;
using Microsoft.AspNetCore.Http;

namespace Analytics.Infrastructure.Auth;

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _tenantId;
    private bool _isResolved;

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            if (!_isResolved)
                ResolveTenantId();
            return _tenantId;
        }
    }
    
    public bool IsAuthenticated  => TenantId.HasValue;
    
    public Guid RequiredTenantId()
    {
        return TenantId 
               ?? throw new UnauthorizedAccessException(
                   "Цей endpoint вимагає автентифікації з TenantId.");
    }
    
    private void ResolveTenantId()
    {
        _isResolved = true;
        
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User.Identity?.IsAuthenticated != true)
        {
            _tenantId = null;
            return;
        }
        
        var claim = httpContext.User.FindFirst("tenantId");
        if (claim is not null && Guid.TryParse(claim.Value, out var tenantId))
            _tenantId = tenantId;
    }
}