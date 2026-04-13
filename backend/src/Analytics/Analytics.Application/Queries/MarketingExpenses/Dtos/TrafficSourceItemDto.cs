namespace Analytics.Application.Queries.MarketingExpenses.Dtos;

public record TrafficSourceItemDto
{
    public string Source { get; init; }
    public int Clicks { get; init; }
    public decimal Spend { get; init; }
    public decimal Revenue { get; init; }
    public decimal Roas { get; init; }
} 