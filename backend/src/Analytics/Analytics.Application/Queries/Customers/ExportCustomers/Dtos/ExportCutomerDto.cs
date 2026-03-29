namespace Analytics.Application.Queries.Customers.ExportCustomers.Dtos;

public class ExportCustomerDto
{
    public Guid CustomerId { get; init; }
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PhoneNumber { get; init; }
    public DateTime RegistrationDate { get; init; }
    public DateTime? FirstOrderDate { get; init; }
    public DateTime? LastOrderDate { get; init; }
    public int OrderCount { get; init; }
    public string Status { get; init; } = default!;
    public string? AcquisitionChannel { get; init; }
    public decimal LifetimeValue { get; init; }
}