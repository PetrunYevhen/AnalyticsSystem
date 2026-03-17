using Domain.Events;

namespace Analytics.Domain.Entities.Transactions.Events;

public class PaymentTransactionRecordedDomainEvent : DomainEventBase
{
    public Guid TenantId { get; }
    public Guid PaymentId { get; }
    public Guid TransactionId { get; }

    public PaymentTransactionRecordedDomainEvent(Guid tenantId, Guid paymentId, Guid transactionId)
    {
        TenantId = tenantId;
        PaymentId = paymentId;
        TransactionId = transactionId;
    }
}