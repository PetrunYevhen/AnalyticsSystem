namespace Analytics.Application.Commands.Orders.CreateOrderManually.Dto;

public class OrderDto
{
    public Guid? CustomerId { get; init; }
    public string? ExternalOrderId { get; init; }
    public DateTime OrderDate { get; init; }
    public required string Status { get; init; }
    public required string Currency { get; init; }
    public decimal? InitialPaymentAmount { get; init; }
    public string? InitialPaymentMethod { get; init; }
    public string? InitialPaymentNote { get; init; }
    public required IReadOnlyList<OrderItemDto> Items  { get; init; }
}