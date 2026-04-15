using Analytics.Application.Commands.Transactions.CancelTransaction;
using Analytics.Application.Commands.Transactions.ImportTransactions;
using Analytics.Application.Commands.Transactions.RecordTransaction;
using Analytics.Application.Commands.Transactions.RefundTransaction;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Transactions.ExportTransactions;
using Analytics.Application.Queries.Transactions.GetOrderPayment;
using Analytics.Application.Queries.Transactions.GetTransactionsDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]

public class TransactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionsDashboard(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] TransactionSortFields sortBy = TransactionSortFields.TransactionDate,
        [FromQuery] SortDirection direction = SortDirection.Desc,
        [FromQuery] string? search = null)
    {
        var query = new GetTransactionsDashboardQuery(page, pageSize, sortBy, direction, search);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("{orderId:guid}/payment")]
    public async Task<IActionResult> GetPayment(Guid orderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderPaymentQuery(
            orderId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }

    
    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("{orderId:guid}/payment")]
    public async Task<IActionResult> RecordPayment( Guid orderId,
        [FromBody] TransactionRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new RecordTransactionCommand
        (
            orderId,
            request.Amount,
            request.Method,       
            request.Note
        ), ct);
        
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
    
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var periodResult = Period.ResolvePeriod(from, to);
        if (periodResult.IsFailed)
            return BadRequest(periodResult.Errors);

        var query = new ExportTransactionsQuery(periodResult.Value);
        var bytes = await _mediator.Send(query);

        return File(bytes, "text/csv", $"transactions_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
    }

    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("{orderId:guid}/payment/refund")]
    public async Task<IActionResult> RecordRefund(Guid orderId, 
        [FromBody] TransactionRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new RefundTransactionCommand(orderId, request.Amount, request.Note, request.Method), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
    
    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("{orderId:guid}/payment/cancel")]
    public async Task<IActionResult> CancelPayment(
        Guid orderId,
        [FromBody] CancelPaymentRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelTransactionCommand(orderId, request.Note), ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
    
    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Import([FromForm] IFormFile? file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Файл порожній");

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Тільки CSV файли");

        var command = new ImportTransactionsCommand(file);
        var result = await _mediator.Send(command, ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
    
    public record TransactionRequest(decimal Amount, string Method, string? Note);
    public record CancelPaymentRequest(string? Note);

}