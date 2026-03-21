using Analytics.Application.Caching;
using Analytics.Domain.Entities.Marketing.Events;
using MediatR;

namespace Analytics.Application.DomainEventHandlers.Marketing;

public class InvalidateCacheOnCampaignCreated : INotificationHandler<CampaignCreatedDomainEvent>
{
    private readonly ICacheService _cacheService;
    private readonly CacheKeys _cacheKeys;

    public InvalidateCacheOnCampaignCreated(ICacheService cacheService, CacheKeys cacheKeys)
    {
        _cacheService = cacheService;
        _cacheKeys = cacheKeys;
    }

    public async Task Handle(CampaignCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _cacheService.RemoveByPrefixAsync(_cacheKeys.Resolve("campaigns:"), cancellationToken);
    }
}