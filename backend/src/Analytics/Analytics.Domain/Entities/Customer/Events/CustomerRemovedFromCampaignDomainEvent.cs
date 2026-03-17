using Domain.Events;

namespace Analytics.Domain.Entities.Customer.Events;

public class CustomerRemovedFromCampaignDomainEvent : DomainEventBase
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? CampaignId { get;  set; }
    
    public CustomerRemovedFromCampaignDomainEvent(Guid customerId, Guid? campaignId, Guid tenantId)
    {
        CustomerId = customerId;
        CampaignId = campaignId;
        TenantId = tenantId;
    }
}