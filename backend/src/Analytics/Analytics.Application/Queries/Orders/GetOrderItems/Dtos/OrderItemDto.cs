namespace Analytics.Application.Queries.Orders.GetOrderItems.Dtos;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductCategory { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal UnitCost { get; set; }
    public string Currency { get; set; }
    
}