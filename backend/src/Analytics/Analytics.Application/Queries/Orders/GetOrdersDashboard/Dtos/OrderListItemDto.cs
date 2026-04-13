namespace Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

public record OrderListItemDto(
    string OrderNumber,   
    string CustomerName,
    DateTime OrderDate,   
    string Status,
    decimal TotalAmount
);