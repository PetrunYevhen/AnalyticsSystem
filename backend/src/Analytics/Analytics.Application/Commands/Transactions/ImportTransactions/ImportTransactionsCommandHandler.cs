using System.Data;
using Analytics.Application.Auth;
using Analytics.Application.Commands.Transactions.ImportTransactions.Dtos;
using Analytics.Application.Common.Csv;
using Analytics.Domain.Entities.Transactions.Enums;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Transactions.ImportTransactions;

public class ImportTransactionsCommandHandler 
    : IRequestHandler<ImportTransactionsCommand, Result<ImportTransactionsResult>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ImportTransactionsCommandHandler(
        INpgsqlConnectionFactory connectionFactory,
        ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<ImportTransactionsResult>> Handle(
        ImportTransactionsCommand request, CancellationToken ct)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        var rows = ImportCsv.ParseCsv<ImportTransactionsCsvRow>(request.File);
        if (rows.IsFailed)
            return Result.Fail(rows.Errors);

        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        foreach (var (row, index) in rows.Value.Select((r, i) => (r, i + 2)))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.ExternalOrderId))
                {
                    errors.Add($"Рядок {index}: ExternalOrderId порожній");
                    skipped++;
                    continue;
                }

                if (row.Amount <= 0)
                {
                    errors.Add($"Рядок {index}: Amount має бути більше 0");
                    skipped++;
                    continue;
                }

                if (!Enum.TryParse<TransactionType>(row.Type, out var transactionType))
                {
                    errors.Add($"Рядок {index}: невалідний тип транзакції '{row.Type}'");
                    skipped++;
                    continue;
                }

                var orderId = await connection.QuerySingleOrDefaultAsync<Guid?>("""
                    SELECT "Id" FROM "Analytics"."Orders"
                    WHERE "TenantId" = @TenantId AND "ExternalOrderId" = @ExternalOrderId
                    """,
                    new { TenantId = tenantId, row.ExternalOrderId });

                if (!orderId.HasValue)
                {
                    errors.Add($"Рядок {index}: замовлення '{row.ExternalOrderId}' не знайдено");
                    skipped++;
                    continue;
                }

                var paymentId = await ResolvePaymentAsync(connection, tenantId, orderId.Value, row.Amount);

                await connection.ExecuteAsync("""
                    INSERT INTO "Analytics"."PaymentTransactions"
                        ("Id", "TenantId", "PaymentId", "Amount", "Currency", "Method", "Type", "Note", "PaidAt", "CreatedAt")
                    VALUES
                        (@Id, @TenantId, @PaymentId, @Amount, @Currency, @Method, @Type, @Note, @PaidAt, @CreatedAt)
                    ON CONFLICT ("Id") DO NOTHING
                    """,
                    new
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        PaymentId = paymentId,
                        Amount = row.Amount,
                        Currency = "UAH",
                        Method = row.Method,
                        Type = transactionType.ToString(),
                        Note = row.Note,
                        PaidAt = row.PaidAt,
                        CreatedAt = DateTime.UtcNow,
                    });

                await connection.ExecuteAsync("""
                    UPDATE "Analytics"."Payments"
                    SET "PaidAmount" = "PaidAmount" + @Amount,
                        "Status" = CASE 
                            WHEN "PaidAmount" + @Amount >= "TotalAmount" THEN 'Paid'
                            WHEN "PaidAmount" + @Amount > 0 THEN 'PartiallyPaid'
                            ELSE "Status"
                        END
                    WHERE "Id" = @PaymentId
                    """,
                    new { Amount = row.Amount, PaymentId = paymentId });

                imported++;
            }
            catch (Exception ex)
            {
                errors.Add($"Рядок {index}: {ex.Message}");
                skipped++;
            }
        }

        return Result.Ok(new ImportTransactionsResult(imported, skipped, errors));
    }

    private static async Task<Guid> ResolvePaymentAsync(
        IDbConnection connection,
        Guid tenantId,
        Guid orderId,
        decimal amount)
    {
        var existingId = await connection.QuerySingleOrDefaultAsync<Guid?>("""
            SELECT "Id" FROM "Analytics"."Payments"
            WHERE "TenantId" = @TenantId AND "OrderId" = @OrderId
            """,
            new { TenantId = tenantId, OrderId = orderId });

        if (existingId.HasValue)
            return existingId.Value;

        var newId = Guid.NewGuid();
        await connection.ExecuteAsync("""
                                      INSERT INTO "Analytics"."Payments"
                                          ("Id", "TenantId", "OrderId", "TotalAmount", "TotalCurrency", "PaidAmount", "PaidCurrency", "Status", "CreatedAt")
                                      VALUES
                                          (@Id, @TenantId, @OrderId, @TotalAmount, @TotalCurrency, 0, @PaidCurrency, @Status, @CreatedAt)
                                      """,
            new
            {
                Id = newId,
                TenantId = tenantId,
                OrderId = orderId,
                TotalAmount = amount,
                TotalCurrency = "UAH",
                PaidCurrency = "UAH",
                Status = PaymentStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow,
            });

        return newId;
    }
}