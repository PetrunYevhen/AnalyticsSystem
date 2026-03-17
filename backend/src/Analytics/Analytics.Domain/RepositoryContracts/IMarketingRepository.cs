using Analytics.Domain.Entities.Marketing;

namespace Analytics.Domain.RepositoryContracts;

public interface IMarketingRepository
{
    Task AddAsync(MarketingExpense marketingExpense, CancellationToken cancellationToken);
}