using Analytics.Application.Queries.Orders.GetOrdersDashboard.Dtos;

namespace Analytics.Application.Queries.Transactions.GetTransactiosDashboard.Dtos;

public record TransactionsDashboardDto(
         TransactionStatsDto Stats,
         List<TransactionItemDto> Transactions);