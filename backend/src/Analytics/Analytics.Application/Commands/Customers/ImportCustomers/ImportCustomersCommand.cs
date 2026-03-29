using Analytics.Application.Commands.Customers.ImportCustomers.Dtos;
using Analytics.Application.Contracts;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Analytics.Application.Commands.Customers.ImportCustomers;

public class ImportCustomersCommand : CommandBase<Result<ImportCustomersResult>>
{
    public ImportCustomersCommand(IFormFile? file)
    {
        File = file;
    }

    public IFormFile? File { get; set; }
}