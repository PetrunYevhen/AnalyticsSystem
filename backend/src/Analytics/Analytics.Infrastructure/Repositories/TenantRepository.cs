using Analytics.Domain.Entities.Tenant;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public TenantRepository(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tenant> AddAsync(Tenant tenant)
    {
        await _dbContext.Tenants.AddAsync(tenant);
        return tenant;
    }

    public Task<Tenant> UpdateAsync(Tenant tenant)
    {
        _dbContext.Tenants.Update(tenant);
        return Task.FromResult(tenant);
    }

    public async Task<Tenant?> GetByApiPrefixAsync(string apiPrefix)
    {
        var tenant = await _dbContext.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ApiKeyPrefix == apiPrefix);

        return tenant;
    }
}