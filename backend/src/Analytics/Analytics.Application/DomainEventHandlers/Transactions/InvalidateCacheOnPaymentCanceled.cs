using Analytics.Application.Caching;
using Analytics.Domain.Entities.Transactions.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Transactions;

public class InvalidateCacheOnPaymentCanceled
    : INotificationHandler<PaymentCanceledDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;

    public InvalidateCacheOnPaymentCanceled(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys = cacheKeys;
    }

    public async Task Handle(PaymentCanceledDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
                _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("dashboard:"), cancellationToken),
                _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("transactions-dashboard:"), cancellationToken), 
                _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("customers:"), cancellationToken)
        );
    }
}