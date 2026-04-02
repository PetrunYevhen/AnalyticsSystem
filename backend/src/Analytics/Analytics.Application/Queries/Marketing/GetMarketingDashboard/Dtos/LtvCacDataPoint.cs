namespace Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;

public class LtvCacDataPoint
{
    public string Date { get; init; } = default!;
    public decimal Ltv { get; init; }
    public decimal Cac { get; init; }
}