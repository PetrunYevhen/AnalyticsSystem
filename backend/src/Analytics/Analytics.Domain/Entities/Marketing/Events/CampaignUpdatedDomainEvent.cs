using Domain.Events;

namespace Analytics.Domain.Entities.Marketing.Events;

public class CampaignUpdatedDomainEvent : DomainEventBase
{
    public CampaignUpdatedDomainEvent(Guid campaignId, Guid tenantId)
    {
        CampaignId = campaignId;
        TenantId = tenantId;
    }

    public Guid CampaignId { get; set; }
    public Guid TenantId { get; set; }
}