using Domain.Events;

namespace Analytics.Domain.Entities.Marketing.Events;

public class CampaignStatusChangedDomainEvent : DomainEventBase
{
    public CampaignStatusChangedDomainEvent(Guid tenantId, Guid campaignId, string newStatus)
    {
        TenantId = tenantId;
        CampaignId = campaignId;
        NewStatus = newStatus;
    }

    public Guid TenantId  { get; }
    public Guid CampaignId { get; }
    public string NewStatus { get; }    
}