using System.Data;
using Analytics.Application.Auth;
using Analytics.Application.Commands.Orders.ImportOrders.Dtos;
using Analytics.Application.Common.Csv;
using Analytics.Domain.Entities.Order;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Orders.ImportOrders;

public class ImportOrdersCommandHandler : IRequestHandler<ImportOrdersCommand, Result<ImportOrdersResult>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ImportOrdersCommandHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<ImportOrdersResult>> Handle(ImportOrdersCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        var rows = ImportCsv.ParseCsv<ImportOrdersCsvRow>(request.File);
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
                if (string.IsNullOrWhiteSpace(row.CustomerEmail))
                {
                    errors.Add($"Рядок {index}: CustomerEmail порожній");
                    skipped++;
                    continue;
                }
                if (!Enum.TryParse<OrderStatus>(row.Status, out var status))
                {
                    errors.Add($"Рядок {index}: невалідний статус '{row.Status}'");
                    skipped++;
                    continue;
                }
                
                var customerId = await ResolveCustomerAsync(connection, tenantId, row);
                
                var isNewOrder = await connection
                    .QuerySingleAsync<bool>("""
                                         SELECT NOT EXISTS (
                                             SELECT 1 FROM "Analytics"."Orders"
                                             WHERE "TenantId" = @TenantId AND "ExternalOrderId" = @ExternalOrderId
                                         )
                                         """,
                    new { TenantId = tenantId, ExternalOrderId = row.ExternalOrderId });

                if (isNewOrder)
                {
                    await connection.ExecuteAsync("""
                                                  UPDATE "Analytics"."Customers"
                                                  SET "OrderCount" = "OrderCount" + 1,
                                                      "LastOrderDate"  = CASE 
                                                          WHEN "LastOrderDate" IS NULL OR "LastOrderDate" < @OrderDate 
                                                          THEN @OrderDate 
                                                          ELSE "LastOrderDate" 
                                                      END,
                                                      "FirstOrderDate" = CASE 
                                                          WHEN "FirstOrderDate" IS NULL OR "FirstOrderDate" > @OrderDate 
                                                          THEN @OrderDate 
                                                          ELSE "FirstOrderDate" 
                                                      END
                                                  WHERE "TenantId" = @TenantId AND "Id" = @CustomerId
                                                  """,
                        new
                        {
                            TenantId = tenantId,
                            CustomerId = customerId,
                            OrderDate = row.OrderDate,
                        });
                }
                
                await connection.ExecuteAsync("""
                                              INSERT INTO "Analytics"."Orders"
                                                  ("Id", "TenantId", "CustomerId", "ExternalOrderId", "OrderDate", "Status", "TotalAmount", "Currency", "CreatedAt")
                                              VALUES
                                                  (@Id, @TenantId, @CustomerId, @ExternalOrderId, @OrderDate, @Status, @TotalAmount, @Currency, @CreatedAt)
                                              ON CONFLICT ("TenantId", "ExternalOrderId")
                                              DO UPDATE SET
                                                  "Status"      = EXCLUDED."Status",
                                                  "TotalAmount" = EXCLUDED."TotalAmount",
                                                  "OrderDate"   = EXCLUDED."OrderDate"
                                              """,
                    new
                    {
                        Id = Guid.NewGuid(),
                        tenantId,
                        CustomerId = customerId,
                        row.ExternalOrderId,
                        row.OrderDate,
                        Status = status.ToString(),
                        row.TotalAmount,
                        Currency = string.IsNullOrWhiteSpace(row.Currency) ? "UAH" : row.Currency,
                        CreatedAt = DateTime.UtcNow,
                    });

                imported++;
            }
            
            catch (Exception ex)
            {
                errors.Add($"Рядок {index}: {ex.Message}");
                skipped++;
            }
        }
        return Result.Ok(new ImportOrdersResult(imported, skipped, errors));
    }

    private static async Task<Guid> ResolveCustomerAsync(
        IDbConnection connection,
        Guid tenantId,
        ImportOrdersCsvRow row)
    {
        string existingIdSql = """
                               SELECT "Id" FROM "Analytics"."Customers"
                               WHERE "TenantId" = @TenantId AND "Email" = @Email
                               """;

        var existingId = await connection.QuerySingleOrDefaultAsync<Guid?>(existingIdSql,
            new
            {
                TenantId = tenantId,
                Email = row.CustomerEmail
            });
        
        if (existingId.HasValue)
            return existingId.Value;

        var newId = Guid.NewGuid();
        string newCustomerSql = """
                                INSERT INTO "Analytics"."Customers"
                                    ("Id", "TenantId", "ExternalId", "FullName", "Email", "RegistrationDate", "OrderCount", "Status", "AcquisitionChannel", "TotalRevenue", "CreatedAt")
                                VALUES
                                    (@Id, @TenantId, @ExternalId, @FullName, @Email, @RegistrationDate, 0, @Status, 'Import', 0, @CreatedAt)
                                """;

        await connection.ExecuteAsync(newCustomerSql,
            new
            {
                Id = newId,
                TenantId = tenantId,
                ExternalId = Guid.NewGuid().ToString(),
                FullName = string.IsNullOrWhiteSpace(row.CustomerFullName) ? row.CustomerEmail : row.CustomerFullName,
                Email = row.CustomerEmail,
                RegistrationDate = DateTime.UtcNow,
                Status = row.Status,
                CreatedAt = DateTime.UtcNow,
            });

        return newId;
    }
}