using Analytics.Domain.Entities.Marketing;

namespace Analytics.Domain.RepositoryContracts;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid tenantId,Guid campaignId, CancellationToken cancellationToken);
    Task AddAsync(Campaign campaign, CancellationToken cancellationToken);
    Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken);
}