namespace Analytics.Application.Queries.Marketing.GetAllCampaigns.Dtos;

public class CampaignDto
{
    public Guid Id { get; init; }        
    public required string Name { get; init; }
    public required string Channel { get; init; }
    public required string Status { get; init; }
    public DateTime ActivePeriodStart { get; init; }
    public DateTime ActivePeriodEnd { get; init; }
    public decimal Budget { get; init; }
    public decimal ActualSpend { get; init; }
    public required string Currency { get; init; }
    public int CustomersCount { get; init; }
}