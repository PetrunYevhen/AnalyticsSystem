namespace Analytics.Application.Commands.Orders.ImportOrders.Dtos;

public record ImportOrdersResult(
    int Imported,
    int Skipped,
    List<string> Errors
    );