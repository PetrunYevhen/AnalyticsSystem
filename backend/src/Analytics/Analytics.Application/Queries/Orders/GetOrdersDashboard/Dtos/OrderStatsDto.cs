namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrderStatsDto
{
    public long TotalOrders { get; init; }       // Обов'язково слово public!
    public long SuccessfulOrders { get; init; }  // Обов'язково слово public!
    public long ProcessingOrders { get; init; }  // Обов'язково слово public!
}