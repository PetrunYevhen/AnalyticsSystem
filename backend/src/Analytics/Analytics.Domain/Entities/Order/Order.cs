using Analytics.Domain.Entities.Order.Events;
using Analytics.Domain.Exceptions;
using Domain;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Order;
public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public string? ExternalOrderId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    
    private readonly List<OrderItem> _items = new(); 
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private Order(){}

    public static Order Create(
        Guid tenantId, 
        Guid customerId, 
        string? externalOrderId, 
        Currency currency, 
        DateTime orderDate, 
        OrderStatus status
        )
    {
        if(customerId == Guid.Empty)
            throw new ArgumentException($"{nameof(customerId)} не може бути пустий");

        var normalizedExternalId = string.IsNullOrWhiteSpace(externalOrderId) ? null : externalOrderId.Trim();
        
        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            CustomerId = customerId,
            ExternalOrderId = normalizedExternalId,
            TotalAmount = new Money(0, currency),
            OrderDate = orderDate,
            Status = status,
            CreatedAt = DateTime.UtcNow,
        };

        order.AddDomainEvent(new OrderCreatedDomainEvent(
            order.TenantId,
            order.Id));
        
        return order;
    }
    
    public void AddItem(
        string productExternalId,
        string productName,
        string category,
        int quantity,
        decimal unitPrice,
        decimal unitCost)
    {
        if (quantity <= 0)
            throw new DomainException("Кількість не може бути 0 або менша");

        if (unitPrice < 0)
            throw new DomainException("Ціна не може бути від'ємною");

        if (unitCost < 0)
            throw new DomainException("Собівартість не може бути від'ємною");

        _items.Add(new OrderItem(
            Id,
            productExternalId,
            productName,
            category,
            quantity,
            unitPrice,
            unitCost,
            TotalAmount.Currency));

        RecalculateTotalAmount();
    }
    
    private void RecalculateTotalAmount()
    {
        var calculatedAmount = _items.Sum(i => i.Quantity * i.UnitPrice);
        TotalAmount = new Money(calculatedAmount, TotalAmount.Currency);
    }
    
    public void MarkAsPaid()
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Не можна оновити статус скасованого замовлення.");
        
        Status = OrderStatus.Completed;
        AddDomainEvent(new OrderStatusChangedDomainEvent(TenantId, Id, Status));
    }
    public void MarkAsCanceled()
    {
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Не можна оновити статус скасованого замовлення.");
    
        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderStatusChangedDomainEvent(TenantId, Id, Status));
    }
}