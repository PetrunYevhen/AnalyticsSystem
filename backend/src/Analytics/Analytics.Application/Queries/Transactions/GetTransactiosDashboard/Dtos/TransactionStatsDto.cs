namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record TransactionStatsDto
{
    public decimal AvailableBalance { get; init; }
    public decimal YesterdayRevenue { get; init; }
    public decimal RefundedAmount { get; init; }
};