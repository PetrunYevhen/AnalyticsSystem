namespace Analytics.Application.Commands.Transactions.ImportTransactions.Dtos;

public class ImportTransactionsCsvRow
{
    public Guid TransactionId { get; set; }
    public Guid PaymentId { get; set; }
    public string ExternalOrderId { get; set; } = default!;
    public string CustomerName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public string Type { get; set; } = default!;
    public string? Note { get; set; }
    public DateTime PaidAt { get; set; }
}