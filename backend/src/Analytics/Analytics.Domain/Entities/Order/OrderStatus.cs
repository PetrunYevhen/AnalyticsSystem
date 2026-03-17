namespace Analytics.Domain.Entities.Order; 

public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Completed,
    Cancelled,
}