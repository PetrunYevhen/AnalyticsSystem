namespace Analytics.Application.Queries.Customers.GetAllCustomers;

public static class SortColumnMap
{
    public static readonly IReadOnlyDictionary<CustomerSortFields, string> Columns
        = new Dictionary<CustomerSortFields, string>
        {
            [CustomerSortFields.FullName] = "\"FullName\"",
            [CustomerSortFields.Email] = "\"Email\"",
            [CustomerSortFields.LastOrderDate] = "\"LastOrderDate\"",
            [CustomerSortFields.Status] = "\"Status\"",
            [CustomerSortFields.AcquisitionChannel] = "\"AcquisitionChannel\"",
            [CustomerSortFields.TotalRevenue] = "\"TotalRevenue\""
        };
}