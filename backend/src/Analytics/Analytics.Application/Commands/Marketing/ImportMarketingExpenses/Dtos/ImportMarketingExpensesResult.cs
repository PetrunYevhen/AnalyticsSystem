namespace Analytics.Application.Commands.Marketing.ImportMarketingExpenses.Dtos;

public record ImportMarketingExpensesResult(
    int Imported,
    int Skipped,
    List<string>Errors);
