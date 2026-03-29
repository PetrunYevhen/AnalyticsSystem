namespace Analytics.Application.Queries.Transactions.GetTransactionsDashboard.Dtos;

public record TransactionItemDto(
    string ExternalOrderId,
    string CustomerName,
    string Method,
    DateTime Date,
    string Status,
    string TransactionType,
    decimal Amount);