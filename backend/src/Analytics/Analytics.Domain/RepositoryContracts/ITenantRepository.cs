using Analytics.Domain.Entities.Tenant;

namespace Analytics.Domain.RepositoryContracts;

public interface ITenantRepository
{
    Task<Tenant> AddAsync(Tenant tenant);
    Task<Tenant> UpdateAsync(Tenant tenant);
}