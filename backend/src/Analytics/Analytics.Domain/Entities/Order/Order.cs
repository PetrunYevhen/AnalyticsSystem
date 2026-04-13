using Domain;

namespace Analytics.Domain.Entities.Order;
// Entity to calculate customer LTV 
public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public string ExternalOrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public DateTime OrderDate { get; private set; }
    public OrderStatus Status { get; private set; }
    public string? CustomAttributesJson { get; private set; }
    private readonly List<OrderItem> _items = new(); 
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private Order(){}
    
    
    public Order(Guid tenantId, Guid customerId, string externalOrderId, decimal totalAmount, string currency,
        DateTime orderDate,OrderStatus orderStatus, string? customAttributesJson = null)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerId = customerId;
        ExternalOrderId = externalOrderId;
        TotalAmount = totalAmount;
        Currency = currency;
        OrderDate = orderDate;
        Status = orderStatus;
        CustomAttributesJson = customAttributesJson;
        CreatedAt = DateTime.UtcNow;
    }
    public void AddItem(string productId, string name, string category, int quantity, decimal price, decimal cost)
     {
         _items.Add(new OrderItem(this.Id, productId, name, category, quantity, price, cost));
     }
}