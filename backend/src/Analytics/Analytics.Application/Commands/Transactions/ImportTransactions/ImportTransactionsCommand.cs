using Analytics.Application.Commands.Transactions.ImportTransactions.Dtos;
using Analytics.Application.Contracts;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Analytics.Application.Commands.Transactions.ImportTransactions;

public class ImportTransactionsCommand : CommandBase<Result<ImportTransactionsResult>>
{
    public ImportTransactionsCommand(IFormFile file)
    {
        File = file;
    }

    public IFormFile File { get; set; }
}