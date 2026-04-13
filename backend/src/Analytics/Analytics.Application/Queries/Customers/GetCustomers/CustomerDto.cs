
namespace Analytics.Application.Queries.Customers.GetCustomers;

public record CustomerDto(
     Guid Id,
     string FullName,
     string? Email,
     string Status,
     DateTime LastOrderDate,
     string AcquisitionChannel
);
