using Domain.Events;

namespace Analytics.Domain.Entities.Customer.Events;

public class CustomerAssignedToCampaignDomainEvent : DomainEventBase
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CampaignId { get;  set; }
    
    public CustomerAssignedToCampaignDomainEvent(Guid customerId, Guid campaignId, Guid tenantId)
    {
        CustomerId = customerId;
        CampaignId = campaignId;
        TenantId = tenantId;
    }
}