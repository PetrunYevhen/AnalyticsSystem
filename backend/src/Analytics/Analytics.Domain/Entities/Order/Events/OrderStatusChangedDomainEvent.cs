using Domain.Events;

namespace Analytics.Domain.Entities.Order.Events;

public class OrderStatusChangedDomainEvent : DomainEventBase
{
    public OrderStatusChangedDomainEvent(Guid tenantId, Guid orderId, OrderStatus orderStatus)
    {
        TenantId = tenantId;
        OrderId = orderId;
        OrderStatus = orderStatus;
    }

    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    public OrderStatus OrderStatus { get; set; }
}