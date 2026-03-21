using Analytics.Application.Caching;
using Analytics.Domain.Entities.Transactions.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Transactions;

public class InvalidateCacheOnPaymentTransactionRecorded
    : INotificationHandler<PaymentTransactionRecordedDomainEvent>
{
    private readonly ICacheService _cacheService;

    public InvalidateCacheOnPaymentTransactionRecorded(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }

    public async Task Handle(PaymentTransactionRecordedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _cacheService.RemoveByPrefixAsync($"tenant:{notification.TenantId}:transactions-dashboard:", cancellationToken),
            _cacheService.RemoveByPrefixAsync($"tenant:{notification.TenantId}:dashboard:", cancellationToken)
        );
    }
}