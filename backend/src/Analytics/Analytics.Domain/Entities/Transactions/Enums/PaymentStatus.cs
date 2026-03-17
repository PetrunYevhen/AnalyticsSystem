namespace Analytics.Domain.Entities.Transactions.Enums;

public enum PaymentStatus
{
    Pending,
    PartiallyPaid,
    Paid,
    Refunded,
    Cancelled
}