using Domain.Events;

namespace Analytics.Domain.Entities.Marketing.Events;

public class CampaignCreatedDomainEvent : DomainEventBase
{
    public CampaignCreatedDomainEvent(Guid tenantId, Guid campaignId)
    {
        TenantId = tenantId;
        CampaignId = campaignId;
    }

    public Guid TenantId { get; set; }
    public Guid CampaignId { get; set; }
}