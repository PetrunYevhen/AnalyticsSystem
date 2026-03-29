using Analytics.Application.Commands.Orders.ImportOrders.Dtos;
using Analytics.Application.Contracts;
using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Analytics.Application.Commands.Orders.ImportOrders;

public class ImportOrdersCommand : CommandBase<Result<ImportOrdersResult>>
{
    public ImportOrdersCommand(IFormFile? file)
    {
        File = file;
    }

    public IFormFile? File  { get; set; }
}