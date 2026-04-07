using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class CampaignRepository : ICampaignRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public CampaignRepository(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Campaign?> GetByIdAsync(Guid tenantId, Guid campaignId, CancellationToken cancellationToken)
    {
        return await  _dbContext.Campaigns.FirstOrDefaultAsync(
            c => c.TenantId == tenantId 
                 && c.Id == campaignId, cancellationToken);
    }

    public async Task AddAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        await _dbContext.Campaigns.AddAsync(campaign, cancellationToken);
    }

    public Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken)
    {
        _dbContext.Campaigns.Update(campaign);
        return Task.CompletedTask;
    }
}
