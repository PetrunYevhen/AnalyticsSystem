using Analytics.Domain.Entities.Transactions.Enums;
using Domain;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Transactions;

public class PaymentTransaction : Entity
{
        public Guid PaymentId { get; private set; }
        public Money Amount { get; private set; }
        public PaymentMethod? Method { get; private set; }
        public TransactionType Type { get; private set; }  
        public string? Note { get; private set; }
        public DateTime PaidAt { get; private set; }

        internal PaymentTransaction(
            Guid id, Guid tenantId, Guid paymentId,
            Money amount, PaymentMethod? method, TransactionType type, string? note, DateTime paidAt)
        {
            Id = id;
            TenantId = tenantId;
            PaymentId = paymentId;
            Amount = amount;
            Method = method;
            Type = type;
            Note = note;
            PaidAt = paidAt;
            CreatedAt = paidAt;
        }
        internal PaymentTransaction() { }
}