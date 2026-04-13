using Analytics.Application.Contracts;

namespace Analytics.Application.Queries.Tenants.GetTenant;

public class GetTenantQuery : QueryBase<TenantDto> 
{
    public Guid TenantId { get; }

    public GetTenantQuery(Guid tenantId)
    {
        TenantId = tenantId;
    }
}

