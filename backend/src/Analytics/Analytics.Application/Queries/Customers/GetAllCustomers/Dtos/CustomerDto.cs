
namespace Analytics.Application.Queries.Customers.GetAllCustomers.Dtos;

public record CustomerDto(
     Guid Id,
     string FullName,
     string? Email,
     string Status,
     DateTime LastOrderDate,
     string AcquisitionChannel,
     decimal TotalRevenue
);


