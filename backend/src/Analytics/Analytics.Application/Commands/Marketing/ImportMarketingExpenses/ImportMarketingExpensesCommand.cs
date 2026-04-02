using Analytics.Application.Commands.Customers.ImportCustomers.Dtos;
using Analytics.Application.Commands.Marketing.ImportMarketingExpenses.Dtos;
using Analytics.Application.Contracts;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Analytics.Application.Commands.Marketing.ImportMarketingExpenses;

public class ImportMarketingExpensesCommand : CommandBase<Result<ImportMarketingExpensesResult>>
{
    public ImportMarketingExpensesCommand(IFormFile formFile)
    {
        File = formFile;
    }

    public IFormFile File { get; set; }
}