using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.RepositoryContracts;

namespace Analytics.Infrastructure.Repositories;

public class MarketingRepository : IMarketingRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public MarketingRepository(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(MarketingExpense marketingExpense, CancellationToken cancellationToken)
    {
        await _dbContext.MarketingExpenses.AddAsync(marketingExpense, cancellationToken);
    }
}