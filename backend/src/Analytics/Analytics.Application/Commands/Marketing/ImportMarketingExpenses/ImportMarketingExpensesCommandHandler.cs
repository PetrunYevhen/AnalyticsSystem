using Analytics.Application.Auth;
using Analytics.Application.Commands.Marketing.ImportMarketingExpenses.Dtos;
using Analytics.Application.Common.Csv;
using Application;
using Dapper;
using FluentResults;
using MediatR;

namespace Analytics.Application.Commands.Marketing.ImportMarketingExpenses;

public class ImportMarketingExpensesCommandHandler : IRequestHandler<ImportMarketingExpensesCommand, Result<ImportMarketingExpensesResult>>
{
    private readonly ITenantContext _tenantContext;
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public ImportMarketingExpensesCommandHandler(INpgsqlConnectionFactory connectionFactory, ITenantContext tenantContext)
    {
        _connectionFactory = connectionFactory;
        _tenantContext = tenantContext;
    }

    public async Task<Result<ImportMarketingExpensesResult>> Handle(ImportMarketingExpensesCommand request, CancellationToken ct)
    {
        var tenantId = _tenantContext.RequiredTenantId();
        using var connection = _connectionFactory.CreateNewConnection();

        var rows = ImportCsv.ParseCsv<ImportMarketingExpensesCsvRow>(request.File);
        if (rows.IsFailed)
            return Result.Fail(rows.Errors);

        var errors = new List<string>();
        var imported = 0;
        var skipped = 0;

        foreach (var (row, index) in rows.Value.Select((r, i) => (r, i + 2)))
        {
            try
            {
                if (string.IsNullOrWhiteSpace(row.AdSource))
                {
                    errors.Add($"Рядок {index}: AdSource порожній");
                    skipped++;
                    continue;
                }

                if (row.Amount <= 0)
                {
                    errors.Add($"Рядок {index}: Amount має бути більше 0");
                    skipped++;
                    continue;
                }

                if (row.ExpenseDate == default)
                {
                    errors.Add($"Рядок {index}: ExpenseDate порожній");
                    skipped++;
                    continue;
                }
                await connection.ExecuteAsync("""
                                              INSERT INTO "Analytics"."MarketingExpenses"
                                                  ("Id", "TenantId", "AdSource", "Amount", "ExpenseDate",
                                                   "Impressions", "Clicks", "Leads", "Currency", "CreatedAt")
                                              VALUES
                                                  (@Id, @TenantId, @AdSource, @Amount, @ExpenseDate,
                                                   @Impressions, @Clicks, @Leads, @Currency, @CreatedAt)
                                              """,
                    new
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        row.AdSource,
                        row.Amount,
                        ExpenseDate = new DateTime(row.ExpenseDate.Year, row.ExpenseDate.Month, row.ExpenseDate.Day),
                        row.Impressions,
                        row.Clicks,
                        row.Leads,
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

        return Result.Ok(new ImportMarketingExpensesResult(imported, skipped, errors));
    }
}