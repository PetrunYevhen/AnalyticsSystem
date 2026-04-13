using Domain;

namespace Analytics.Domain.Entities.Order;

public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public string ProductExternalId { get; private set; }
    public string ProductName { get; private set; }
    public string Category { get; private set; } // Для аналітики по категоріях
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; } // Ціна продажу
    public decimal UnitCost { get; private set; }  // Собівартість (для маржі!)

    private OrderItem() { }

    internal OrderItem(Guid orderId, string productExternalId, string productName, string category, int quantity, decimal unitPrice, decimal unitCost)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductExternalId = productExternalId;
        ProductName = productName;
        Category = category;
        Quantity = quantity;
        UnitPrice = unitPrice;
        UnitCost = unitCost;
        CreatedAt = DateTime.UtcNow;
    }
}