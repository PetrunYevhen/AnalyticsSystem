namespace Analytics.Application.Commands.Transactions.ImportTransactions.Dtos;

public record ImportTransactionsResult(
    int Imported, 
    int Skipped, 
    List<string> Errors);
