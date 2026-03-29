using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Transactions.GetTransactionsDashboard.Dtos;
using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Transactions.GetTransactionsDashboard;

public class GetTransactionsDashboardQueryHandler
    : IRequestHandler<GetTransactionsDashboardQuery, TransactionsDashboardDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;
    private readonly ITenantContext _tenantContext;

    public GetTransactionsDashboardQueryHandler(
        INpgsqlConnectionFactory connectionFactory,
        ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<TransactionsDashboardDto> Handle(
        GetTransactionsDashboardQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        if (!SortColumnMap.Columns.TryGetValue(request.SortBy, out var sortColumn))
            throw new ArgumentException($"Неможливо відсортувати {request.SortBy}");

        var direction = request.Direction == SortDirection.Asc ? "ASC" : "DESC";

        using var connection = _connectionFactory.CreateNewConnection();

        var search = request.Search is null 
            ? "" : """
             AND (
                 o."ExternalOrderId" ILIKE @SearchPattern OR
                 c."FullName" ILIKE @SearchPattern
             )
             """;
        
        const string statsSql = """
            SELECT
                COALESCE(SUM(CASE WHEN "Status" = 'Completed'
                    THEN "TotalAmount" ELSE 0 END), 0) AS "AvailableBalance",
                COALESCE(SUM(CASE WHEN "Status" = 'Completed'
                    AND DATE("OrderDate") = CURRENT_DATE - INTERVAL '1 day'
                    THEN "TotalAmount" ELSE 0 END), 0) AS "YesterdayRevenue",
                COALESCE(SUM(CASE WHEN "Status" = 'Refunded'
                    THEN "TotalAmount" ELSE 0 END), 0) AS "RefundedAmount"
            FROM "Analytics"."Orders"
            WHERE "TenantId" = @TenantId;
            """;

        var listSql = $"""
                        SELECT
                            COUNT(*) OVER() AS "TotalCount",
                        o."ExternalOrderId" AS "ExternalOrderId",
                        c."FullName" AS "CustomerName",
                        pt."Method" AS "PaymentMethod",
                        pt."PaidAt" AS "TransactionDate",
                        pt."Type" AS "TransactionType",
                        p."Status" AS "Status",
                        pt."Amount" AS "TotalAmount"
                        FROM "Analytics"."PaymentTransactions" pt
                        JOIN "Analytics"."Payments" p ON pt."PaymentId" = p."Id"
                        JOIN "Analytics"."Orders" o ON p."OrderId" = o."Id"
                        LEFT JOIN "Analytics"."Customers" c ON o."CustomerId" = c."Id"
                        WHERE o."TenantId" = @TenantId
                        {search}
                        ORDER BY {sortColumn} {direction} NULLS LAST, pt."Id" ASC
                        LIMIT @PageSize OFFSET @Skip;
                        """;

        var parameters = new
        {
            TenantId = tenantId,
            PageSize = request.PageSize,
            Skip = (request.Page - 1) * request.PageSize,
            SearchPattern = request.Search is null ? null : $"%{request.Search.Trim()}%",

        };

        using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                statsSql + listSql,
                parameters,
                cancellationToken: cancellationToken));

        var stats = await multi.ReadSingleOrDefaultAsync<TransactionStatsDto>()
            ?? new TransactionStatsDto();

        var rows  = (await multi.ReadAsync<TransactionRow>()).AsList();
        var total = rows.FirstOrDefault()?.TotalCount ?? 0;

        return new TransactionsDashboardDto(
            stats,
            new PagedResult<TransactionItemDto>(
                request.Page,
                request.PageSize,
                total,
                rows.Select(r => r.ToDto()).ToList()
            )
        );
    }

    private class TransactionRow
    {
        public string ExternalOrderId { get; init; } = "";
        public required string CustomerName { get; init; } 
        public string PaymentMethod { get; init; } = "";
        public DateTime TransactionDate  { get; init; }
        public string TransactionType  { get; init; } = "";
        public string Status { get; init; } = "";
        public decimal  TotalAmount { get; init; }
        public int TotalCount { get; init; }

        public TransactionItemDto ToDto() =>
            new(ExternalOrderId, CustomerName, PaymentMethod,
                TransactionDate, Status, TransactionType, TotalAmount);
    }
}