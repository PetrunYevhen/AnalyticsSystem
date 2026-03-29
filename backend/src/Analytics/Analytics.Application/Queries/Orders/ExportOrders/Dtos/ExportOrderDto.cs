namespace Analytics.Application.Queries.Orders.ExportOrders.Dtos;

public record ExportOrderDto(
    Guid OrderId,
    string ExternalOrderId,
    string CustomerName,
    string CustomerEmail,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount
);