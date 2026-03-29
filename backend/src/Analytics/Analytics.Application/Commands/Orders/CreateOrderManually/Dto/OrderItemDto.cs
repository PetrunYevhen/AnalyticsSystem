namespace Analytics.Application.Commands.Orders.CreateOrderManually.Dto;

public class OrderItemDto
{
    public string? ProductExternalId { get; init; }
    public required string ProductName  { get; init; }
    public required string Category  { get; init; }
    public int Quantity  { get; init; }
    public decimal UnitCost  { get; init; }
    public decimal UnitPrice  { get; init; }
    
}