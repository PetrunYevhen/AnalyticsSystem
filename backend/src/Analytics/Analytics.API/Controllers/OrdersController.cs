using Analytics.Application.Commands.Orders.CancelOrder;
using Analytics.Application.Commands.Orders.CreateOrderManually;
using Analytics.Application.Commands.Orders.ImportOrders;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Orders.ExportOrders;
using Analytics.Application.Queries.Orders.GetOrderItems;
using Analytics.Application.Queries.Orders.GetOrdersDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]

public class OrdersController : ControllerBase
{ 
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersDashboard([FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] OrderSortFields sortBy = OrderSortFields.OrderDate,
        [FromQuery] SortDirection direction = SortDirection.Desc,
        [FromQuery] string? search = null)
    {
        var result = await _mediator.Send(
            new GetOrdersDashboardQuery(page, pageSize, sortBy, direction, search));
        return Ok(result);
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var periodResult = Period.ResolvePeriod(from, to);
        if (periodResult.IsFailed)
            return BadRequest(periodResult.Errors);

        var query = new ExportOrdersQuery(periodResult.Value);
        var bytes = await _mediator.Send(query);

        return File(bytes, "text/csv", $"orders_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
    }
    
    [HttpGet("{orderId:guid}/items")]
    public async Task<IActionResult> GetOrderItems(Guid orderId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrderItemsQuery(orderId), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
    
    
    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] CreateOrderManuallyCommand order)
    {
        var result = await _mediator.Send(order);
        return Ok(result.Value);
    }

    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid orderId,
        [FromBody] CancelOrderRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new CancelOrderCommand(orderId, request?.Note), ct);
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
    
        var command = new ImportOrdersCommand(file);
        var result = await _mediator.Send(command, ct);
    
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
    
    public record CancelOrderRequest(string? Note);

}