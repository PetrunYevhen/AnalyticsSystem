using Analytics.Application.Caching;
using Analytics.Domain.Entities.Marketing.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Marketing;

public class InvalidateCacheOnCampaignUpdated : INotificationHandler<CampaignUpdatedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;

    public InvalidateCacheOnCampaignUpdated(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys = cacheKeys;
    }

    public async Task Handle(CampaignUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await Task.WhenAll(
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("campaigns:"), cancellationToken),
            _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("marketing-dashboard:"), cancellationToken)
        );
    }
}