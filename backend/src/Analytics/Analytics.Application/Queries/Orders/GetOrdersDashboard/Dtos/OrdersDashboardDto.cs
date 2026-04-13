namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public record OrdersDashboardDto(
    OrderStatsDto OrderStats,
    List<OrderListItemDto> OrderItems 
   );