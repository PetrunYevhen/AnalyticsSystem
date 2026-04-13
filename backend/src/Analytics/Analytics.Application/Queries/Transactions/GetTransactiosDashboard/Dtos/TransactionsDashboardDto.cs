namespace Analytics.Application.Queries.Transactions.Dtos;

public record TransactionsDashboardDto(
         decimal AvailableBalance,
         decimal YesterdayRevenue,
         decimal RefundedAmount,
         List<TransactionItemDto> Transactions);