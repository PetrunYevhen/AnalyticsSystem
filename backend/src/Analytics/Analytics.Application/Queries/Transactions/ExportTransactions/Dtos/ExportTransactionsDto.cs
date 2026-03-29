namespace Analytics.Application.Queries.Transactions.ExportTransactions.Dtos;

public record ExportTransactionsDto(
        Guid TransactionId,
        Guid PaymentId,
        string ExternalOrderId,
        string CustomerName,
        decimal Amount,
        string? Method,
        string Type,
        string? Note,
        DateTime PaidAt);