namespace Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign.Dtos;

public class CampaignCustomerDto
{
    public Guid Id { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string AcquisitionChannel  { get; set; }
}