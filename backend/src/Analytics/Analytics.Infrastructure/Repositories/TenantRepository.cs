using Analytics.Domain.Entities.Tenant;
using Analytics.Domain.RepositoryContracts;

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
        await  _dbContext.SaveChangesAsync();
        return tenant;
    }

    public Task<Tenant> UpdateAsync(Tenant tenant)
    {
        throw new NotImplementedException();
    }
}