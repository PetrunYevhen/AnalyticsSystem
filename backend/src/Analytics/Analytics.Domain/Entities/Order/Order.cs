using Domain;

namespace Analytics.Domain.Entities;
// Entity to calculate customer LTV 
public class Order : Entity
{
    public Guid CustomerId { get; private set; }
    public string ExternalOrderId { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; }
    public DateTime OrderDate { get; private set; }
    public string? CustomAttributesJson { get; private set; }

    private Order(){}
    
    public Order(Guid tenantId, Guid customerId, string externalOrderId, decimal totalAmount, string currency,
        DateTime orderDate, string? customAttributesJson = null)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CustomerId = customerId;
        ExternalOrderId = externalOrderId;
        TotalAmount = totalAmount;
        Currency = currency;
        OrderDate = orderDate;
        CustomAttributesJson = customAttributesJson;
        CreatedAt = DateTime.UtcNow;
    }
}