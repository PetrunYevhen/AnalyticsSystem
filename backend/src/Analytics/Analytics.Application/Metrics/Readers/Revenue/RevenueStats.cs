namespace Analytics.Application.Metrics.Readers.Revenue;

public sealed class RevenueStats
{
    public decimal TotalRevenue { get; init; }
    public int OrdersCount { get; init; }
    public int UniqueCustomers { get; init; }
}