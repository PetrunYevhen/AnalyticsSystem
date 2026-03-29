namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrderListItemDto(
    Guid OrderId,
    string ExternalOrderId,   
    string CustomerName,
    DateTime OrderDate,   
    string Status,
    decimal TotalAmount
);