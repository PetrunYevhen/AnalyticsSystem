using Domain;
using ValueObjects.ValueObject;

namespace Analytics.Domain.Entities.Order;

public sealed class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public string? ProductExternalId { get; private set; } = default!;
    public string ProductName { get; private set; } = default!;
    public string Category { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal UnitCost { get; private set; }
    public Currency Currency { get; private set; }  

    public decimal LineMargin => (UnitPrice - UnitCost) * Quantity;
    public decimal LineRevenue => UnitPrice * Quantity;

    private OrderItem() { }

    public static OrderItem Create(
        string productExternalId,
        string productName,
        string category,
        int quantity,
        decimal unitPrice,
        decimal unitCost,
        Currency currency)
    {
        var orderItem = new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductExternalId = productExternalId,
            ProductName = productName.Trim(),
            Category =  category.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            UnitCost = unitCost,
            Currency = currency
        };
        return orderItem;

    }

    internal OrderItem(
        Guid orderId,
        string productExternalId,
        string productName,
        string category,
        int quantity,
        decimal unitPrice,
        decimal unitCost,
        Currency currency)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductExternalId = productExternalId.Trim();
        ProductName = productName.Trim();
        Category = category.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        UnitCost = unitCost;
        Currency = currency;
    }
}