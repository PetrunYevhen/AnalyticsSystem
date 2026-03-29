using Analytics.Domain.Enums;

namespace Analytics.Application.Commands.Orders.CreateOrderManually.Dto;

public class CustomerDto
{
    public string? ExternalId { get; init; }
    public string? FullName { get; init; }
    public string Email { get; init; }
    public bool IsNewCustomer  { get; init; }
    public string? PhoneNumber { get; init; }
    public DateTime? RegistrationDate  { get; init; }
    public AcquisitionChannel? AcquisitionChannel  { get; init; }
    public Guid? CampaignId  { get; init; }
}