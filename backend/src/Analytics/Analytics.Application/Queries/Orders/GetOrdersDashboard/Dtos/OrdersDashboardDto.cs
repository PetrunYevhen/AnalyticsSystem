namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrdersDashboardDto(
    OrderStatsDto OrderStats,
    List<OrderListItemDto> OrderItems 
   );