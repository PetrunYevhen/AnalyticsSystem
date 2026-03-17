using Domain.Events;

namespace Analytics.Domain.Entities.Order.Events;

public class OrderCreatedDomainEvent : DomainEventBase
{
    public Guid TenantId { get; }
    public Guid OrderId { get; }
    
    public OrderCreatedDomainEvent(Guid tenantId, Guid orderId)
    {
        TenantId = tenantId;
        OrderId = orderId;
    }
}