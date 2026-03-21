using Analytics.Application.Auth;

namespace Analytics.Application.Caching;

public class CacheKeys
{
    private readonly ITenantContext _tenantContext;

    public CacheKeys(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }
    
    private Guid TenantId => _tenantContext.RequiredTenantId();
    
    public string Resolve(string keyIdentifier) => $"tenant:{TenantId}:{keyIdentifier}";
}