using Analytics.Application.Caching;
using Analytics.Domain.Entities.Customer.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Customers;

public class InvalidateCacheOnCustomerRemovedFromCampaign
    : INotificationHandler<CustomerRemovedFromCampaignDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;


    public InvalidateCacheOnCustomerRemovedFromCampaign(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys = cacheKeys;
    }

    public async Task Handle(CustomerRemovedFromCampaignDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("campaigns:"), cancellationToken),
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("customers:"), cancellationToken)
        );
    }
}