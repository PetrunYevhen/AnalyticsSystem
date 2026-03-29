namespace Analytics.Application.Commands.Customers.ImportCustomers.Dtos;

public record ImportCustomersResult(
    int Imported,
    int Skipped,
    List<string> Errors);