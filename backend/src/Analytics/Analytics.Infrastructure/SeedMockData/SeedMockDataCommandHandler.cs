using Application.Tests;
using MediatR;

namespace Analytics.Infrastructure.SeedMockData;

public class SeedMockDataCommandHandler : IRequestHandler<SeedMockDataCommand, bool>
{
    private readonly AnalyticsDbContext _dbContext;

    public SeedMockDataCommandHandler(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Handle(SeedMockDataCommand request, CancellationToken cancellationToken)
    {
        var mockData = MockDataGenerator.Generate(request.TenantId, request.CustomerCount);

        await _dbContext.Customers.AddRangeAsync(mockData.Customers, cancellationToken);
        await _dbContext.Orders.AddRangeAsync(mockData.Orders, cancellationToken);
        await _dbContext.MarketingExpenses.AddRangeAsync(mockData.MarketingExpenses, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}