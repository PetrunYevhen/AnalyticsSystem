namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrderStatsDto
{
    public long TotalOrders { get; init; }       
    public long SuccessfulOrders { get; init; } 
    public long ProcessingOrders { get; init; }  
}