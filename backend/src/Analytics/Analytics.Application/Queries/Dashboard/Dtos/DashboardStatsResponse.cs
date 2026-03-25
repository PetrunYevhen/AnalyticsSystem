namespace Analytics.Application.Queries.Dashboard.Dtos;

public class DashboardStatsResponse
{
    public int PeriodDays { get; set; }

    public StatItemDto TotalRevenue { get; set; } = default!;
    public StatItemDto NewCustomers { get; set; } = default!;
    public StatItemDto Activity { get; set; } = default!;

    public List<RevenueDataPointDto> RevenueChart { get; set; } = new();
    public List<RecentTransactionsDto> RecentTransactions { get; set; } = new();
    
}