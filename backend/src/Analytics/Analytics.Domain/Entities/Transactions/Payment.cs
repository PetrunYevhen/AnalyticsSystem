using Analytics.Domain.Entities.Transactions.Enums;
using Analytics.Domain.Entities.Transactions.Events;
using Domain;
using FluentResults;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Transactions;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }
    public Money TotalAmount  { get; private set; }
    public Money PaidAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    
    private readonly List<PaymentTransaction> _transactions = new();
    public IReadOnlyList<PaymentTransaction> Transactions => _transactions.AsReadOnly();

    public decimal RefundedAmount => _transactions
        .Where(t => t.Type == TransactionType.Refund)
        .Sum(t => t.Amount.Amount);

    private Payment(){}

    public static Payment Create(
        Guid tenantId, Guid orderId, Money totalAmount, DateTime now)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = orderId,
            TotalAmount = totalAmount,
            PaidAmount = Money.Zero(totalAmount.Currency),
            Status = PaymentStatus.Pending,
            CreatedAt = now,
        };
    }

    public Result<PaymentTransaction> RecordTransaction(decimal amount, PaymentMethod method, string? note, DateTime now)
    {
        if (Status == PaymentStatus.Cancelled)
            return Result.Fail("Не можна додати транзакцію до скасованого платежу");
        if (Status == PaymentStatus.Refunded)
            return Result.Fail("Не можна додати транзакцію до повернутого платежу");
    
        var money = new Money(amount, TotalAmount.Currency);
        if (PaidAmount.Amount + money.Amount > TotalAmount.Amount)
            return Result.Fail($"Сума перевищує залишок: {TotalAmount.Amount - PaidAmount.Amount} {TotalAmount.Currency.Code}");

        var tx = new PaymentTransaction(
            id: Guid.NewGuid(),
            tenantId: TenantId,
            paymentId: Id,
            amount: money,
            method: method,
            type: TransactionType.Payment,
            note: note?.Trim(),
            paidAt: now);
    
        _transactions.Add(tx);
    
        PaidAmount = new Money(PaidAmount.Amount + money.Amount, TotalAmount.Currency);
        Status = PaidAmount.Amount >= TotalAmount.Amount
            ? PaymentStatus.Paid
            : PaymentStatus.PartiallyPaid;
    
        AddDomainEvent(new PaymentTransactionRecordedDomainEvent(tx.TenantId, tx.PaymentId, tx.Id));
        return Result.Ok(tx);
    }
    public Result<PaymentTransaction> Refund(DateTime now, decimal amount, PaymentMethod method, string? note)
    {
        if (Status != PaymentStatus.Paid && Status != PaymentStatus.PartiallyPaid)
            return Result.Fail("Можна повернути тільки оплачений платіж.");
        
        if (amount <= 0)
            return Result.Fail("Сума повернення має бути більше нуля.");

        if (amount > PaidAmount.Amount - RefundedAmount)
            return Result.Fail($"Сума повернення перевищує доступний залишок: {PaidAmount.Amount - RefundedAmount}");
        
        var tx = new PaymentTransaction(
            id: Guid.NewGuid(),
            tenantId: TenantId,
            paymentId: Id,
            amount: new Money(amount, TotalAmount.Currency),
            method: method,
            type: TransactionType.Refund,
            note: note?.Trim(),
            paidAt: now);
    
        _transactions.Add(tx);
        var netPaid = PaidAmount.Amount - RefundedAmount - amount;
        Status = netPaid <= 0 ? PaymentStatus.Refunded : PaymentStatus.PartiallyPaid;
        return Result.Ok(tx);
    }

    public Result<PaymentTransaction> Cancel(DateTime now, string? note)
    {
        if (Status == PaymentStatus.Paid)
            return Result.Fail("Cannot cancel a paid payment — use Refund.");
    
        if (Status == PaymentStatus.Cancelled)
            return Result.Fail("Payment is already cancelled.");

        var transaction = new PaymentTransaction(
            id: Guid.NewGuid(),
            tenantId: TenantId,
            paymentId: Id,
            amount: TotalAmount - PaidAmount,
            method: PaymentMethod.None, 
            type: TransactionType.Cancellation,
            note: note,
            paidAt: now
        );

        _transactions.Add(transaction);
        Status = PaymentStatus.Cancelled;

        AddDomainEvent(new PaymentCanceledDomainEvent(TenantId, Id, OrderId));
        
        return Result.Ok(transaction);
    }
}