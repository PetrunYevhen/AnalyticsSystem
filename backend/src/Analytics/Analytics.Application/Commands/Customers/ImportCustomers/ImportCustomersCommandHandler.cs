using System.Data;
using Analytics.Application.Auth;
using Analytics.Application.Commands.Customers.ImportCustomers.Dtos;
using Analytics.Application.Common.Csv;
using Analytics.Domain.Entities.Customer;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Customers.ImportCustomers;

public class ImportCustomersCommandHandler : IRequestHandler<ImportCustomersCommand, Result<ImportCustomersResult>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ImportCustomersCommandHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<ImportCustomersResult>> Handle(ImportCustomersCommand request, CancellationToken ct)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        var rows = ImportCsv.ParseCsv<ImportCustomersCsvRow>(request.File);
        if (rows.IsFailed)
            return Result.Fail(rows.Errors);

        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        foreach (var (row, index) in rows.Value.Select((r, i) => (r, i + 2)))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.Email))
                {
                    errors.Add($"Рядок {index}: Email порожній");
                    skipped++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(row.FullName))
                {
                    errors.Add($"Рядок {index}: FullName порожній");
                    skipped++;
                    continue;
                }
                if (!Enum.TryParse<CustomerStatus>(row.Status, out var status))
                {
                    errors.Add($"Рядок {index}: невалідний статус '{row.Status}'");
                    skipped++;
                    continue;
                }

                await connection.ExecuteAsync("""
                                              INSERT INTO "Analytics"."Customers"
                                                  ("Id", "TenantId", "ExternalId", "FullName", "Email", "PhoneNumber",
                                                   "RegistrationDate", "OrderCount", "Status", "AcquisitionChannel", "TotalRevenue", "CreatedAt",
                                                   "FirstOrderDate", "LastOrderDate")
                                              VALUES
                                                  (@Id, @TenantId, @ExternalId, @FullName, @Email, @PhoneNumber,
                                                   @RegistrationDate, 0, @Status, @AcquisitionChannel, 0, @CreatedAt,
                                                   @FirstOrderDate, @LastOrderDate)
                                              ON CONFLICT ("TenantId", "Email")
                                              DO UPDATE SET
                                                  "FullName"           = EXCLUDED."FullName",
                                                  "PhoneNumber"        = EXCLUDED."PhoneNumber",
                                                  "AcquisitionChannel" = EXCLUDED."AcquisitionChannel"
                                              """,
                    new
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        ExternalId = string.IsNullOrWhiteSpace(row.ExternalId) ? Guid.NewGuid().ToString() : row.ExternalId,
                        row.FullName,
                        row.Email,
                        row.PhoneNumber,
                        Status = status.ToString(),
                        RegistrationDate = row.RegistrationDate == default ? DateTime.UtcNow : row.RegistrationDate,
                        AcquisitionChannel = string.IsNullOrWhiteSpace(row.AcquisitionChannel) ? "Import" : row.AcquisitionChannel,
                        CreatedAt = DateTime.UtcNow,
                        FirstOrderDate = row.FirstOrderDate == default ? (DateTime?)null : row.FirstOrderDate,
                        LastOrderDate = row.LastOrderDate == default ? (DateTime?)null : row.LastOrderDate,
                    });

                imported++;
            }
            catch (Exception ex)
            {
                errors.Add($"Рядок {index}: {ex.Message}");
                skipped++;
            }
        }

        return Result.Ok(new ImportCustomersResult(imported, skipped, errors));
    }
}