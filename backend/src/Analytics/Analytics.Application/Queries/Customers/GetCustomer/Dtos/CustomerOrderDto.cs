namespace Analytics.Application.Queries.Customers.GetCustomer.Dtos;

public class CustomerOrderDto
{
    public Guid OrderId { get; init; }
    public string? ExternalOrderId { get; init; }
    public DateTime OrderDate { get; init; }
    public required string Status { get; init; }
    public decimal TotalAmount { get; init; }
}
