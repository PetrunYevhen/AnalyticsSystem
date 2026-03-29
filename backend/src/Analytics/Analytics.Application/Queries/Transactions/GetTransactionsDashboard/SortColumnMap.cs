namespace Analytics.Application.Queries.Transactions.GetTransactionsDashboard;

public static class SortColumnMap
{
    public static readonly IReadOnlyDictionary<TransactionSortFields, string> Columns =
        new Dictionary<TransactionSortFields, string>
        {
            [TransactionSortFields.ExternalOrderId]  = "\"ExternalOrderId\"",
            [TransactionSortFields.CustomerName]     = "\"CustomerName\"",
            [TransactionSortFields.PaymentMethod]    = "\"PaymentMethod\"",
            [TransactionSortFields.TransactionDate]  = "\"TransactionDate\"",
            [TransactionSortFields.TransactionType]  = "\"TransactionType\"",
            [TransactionSortFields.Status]           = "\"Status\"",
            [TransactionSortFields.TotalAmount]      = "\"TotalAmount\"",
        };
}