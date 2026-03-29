namespace Analytics.Application.Commands.Customers.ImportCustomers.Dtos;

public class ImportCustomersCsvRow
{
    public string ExternalId { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Status { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime FirstOrderDate { get; set; }
    public DateTime LastOrderDate { get; set; }
    public string? AcquisitionChannel { get; set; }
}