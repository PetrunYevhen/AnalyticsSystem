
using Analytics.Domain.Entities.Customer;

namespace Analytics.Application.Queries.Customer.GetCustomers;

public record CustomerDto(
     Guid Id,
     string FullName,
     string? Email,
     CustomerStatus Status,
     DateTime LastOrderDate,
     string AcquisitionChannel
);
