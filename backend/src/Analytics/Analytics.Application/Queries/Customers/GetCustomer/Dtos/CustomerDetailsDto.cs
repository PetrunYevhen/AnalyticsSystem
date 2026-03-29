namespace Analytics.Application.Queries.Customers.GetCustomer.Dtos;

public class CustomerDetailsDto
{
    public Guid Id { get; init; }
    public required string FullName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? ExternalId { get; init; }
    public required string Status { get; init; }
    public DateTime RegistrationDate { get; init; }
    public DateTime? FirstOrderDate { get; init; }
    public DateTime? LastOrderDate { get; init; }
    public int OrderCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal PredictedLtv {get; set;}           
    public decimal ArpuMonthly {get; set;} 
    public string? AcquisitionChannel { get; init; }
    public Guid? CampaignId { get; init; }
    public string? CampaignName { get; init; }
    public List<CustomerOrderDto> RecentOrders { get; set; } = new();
}