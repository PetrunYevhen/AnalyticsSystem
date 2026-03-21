using Analytics.Application.Caching;
using Analytics.Domain.Entities.Marketing.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Marketing;

public class InvalidateCacheOnMarketingExpensesCreated : INotificationHandler<MarketingExpensesCreatedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;

    public InvalidateCacheOnMarketingExpensesCreated(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys = cacheKeys;
    }

    public async Task Handle(MarketingExpensesCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("marketing-dashboard:"), cancellationToken);
    }
}