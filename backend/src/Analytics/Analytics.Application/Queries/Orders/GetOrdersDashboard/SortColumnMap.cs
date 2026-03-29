namespace Analytics.Application.Queries.Orders.GetOrdersDashboard;

public static class SortColumnMap
{
    public static readonly IReadOnlyDictionary<OrderSortFields, string> Columns =
        new Dictionary<OrderSortFields, string>
        {
            [OrderSortFields.ExternalOrderId] = "\"ExternalOrderId\"",
            [OrderSortFields.CustomerName] = "\"CustomerName\"",
            [OrderSortFields.OrderDate] = "\"OrderDate\"",
            [OrderSortFields.Status] = "\"Status\"",
            [OrderSortFields.TotalAmount] = "\"TotalAmount\"",
        };
}