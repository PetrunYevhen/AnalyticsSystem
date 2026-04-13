using Analytics.Domain.Entities.Order;

namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public record OrderListItemDto(
    string OrderNumber,   
    string CustomerName,
    DateTime OrderDate,   
    string Status,
    decimal TotalAmount
);