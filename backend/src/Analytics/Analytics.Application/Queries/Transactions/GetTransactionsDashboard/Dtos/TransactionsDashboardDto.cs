using Analytics.Application.Common;

namespace Analytics.Application.Queries.Transactions.GetTransactionsDashboard.Dtos;

public record TransactionsDashboardDto(
         TransactionStatsDto Stats,
         PagedResult<TransactionItemDto> Transactions);