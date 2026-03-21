namespace Analytics.Application.Auth;

public interface ITenantContext
{ 
    Guid? TenantId { get; }
    bool IsAuthenticated { get; }
    Guid RequiredTenantId ();
    
}