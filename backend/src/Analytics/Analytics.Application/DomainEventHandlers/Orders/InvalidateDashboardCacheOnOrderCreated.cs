using Analytics.Application.Caching;
using Analytics.Domain.Entities.Order.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Orders;

public class InvalidateDashboardCacheOnOrderCreated : INotificationHandler<OrderCreatedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;

    public InvalidateDashboardCacheOnOrderCreated(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys    = cacheKeys;
    }

    public async Task Handle(OrderCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("dashboard:"), cancellationToken),
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("orders-dashboard:"), cancellationToken),
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("customers:"), cancellationToken),
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("marketing-dashboard:"), cancellationToken)
        );
    }
}