using Analytics.Application.Common;

namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrdersDashboardDto(
    OrderStatsDto OrderStats,
    PagedResult<OrderListItemDto> OrderItems 
   );