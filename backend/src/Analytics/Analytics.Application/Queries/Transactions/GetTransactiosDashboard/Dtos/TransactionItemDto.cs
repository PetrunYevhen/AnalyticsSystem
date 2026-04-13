namespace Analytics.Application.Queries.Transactions.GetTransactiosDashboard.Dtos;

public record TransactionItemDto(
    string TransactionId,
    string CustomerName,
    string Method,
    DateTime Date,
    string Status,
    decimal Amount);