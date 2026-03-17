using Domain.Events;

namespace Analytics.Domain.Entities.Transactions.Events;

public class PaymentCanceledDomainEvent : DomainEventBase
{
    public PaymentCanceledDomainEvent(Guid tenantId, Guid orderId, Guid paymentId)
    {
        TenantId = tenantId;
        OrderId = orderId;
        PaymentId = paymentId;
    }

    public Guid TenantId { get; set; }
    public Guid OrderId { get; set; }
    public Guid PaymentId { get; set; }
}