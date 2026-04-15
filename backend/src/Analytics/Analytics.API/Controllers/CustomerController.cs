using Analytics.Application.Commands.Customers.ImportCustomers;
using Analytics.Application.Common;
using Analytics.Application.Common.Sort;
using Analytics.Application.Queries.Customers.ExportCustomers;
using Analytics.Application.Queries.Customers.GetAllCustomers;
using Analytics.Application.Queries.Customers.GetByEmail;
using Analytics.Application.Queries.Customers.GetCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Authorize]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] CustomerSortFields sortBy = CustomerSortFields.LastOrderDate,
        [FromQuery] SortDirection direction = SortDirection.Desc,
        [FromQuery] string? search = null,
        [FromQuery] bool? withoutCampaign = null)
    {
        var query = new GetAllCustomersQuery(page, pageSize, sortBy, direction, search, withoutCampaign);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpGet("by-email")]
    public async Task<IActionResult> GetByEmail([FromQuery] string email, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetByEmailQuery(email), ct);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpGet("{customerId:guid}")]
    public async Task<IActionResult> GetCustomerDetails(
        Guid customerId,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCustomerDetailsQuery(customerId), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Errors);
    }
    
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var periodResult = Period.ResolvePeriod(from, to);
        if (periodResult.IsFailed)
            return BadRequest(periodResult.Errors);

        var query = new ExportCustomersQuery(periodResult.Value);
        var bytes = await _mediator.Send(query);

        return File(bytes, "text/csv", $"customers_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
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
    
        var command = new ImportCustomersCommand(file);
        var result = await _mediator.Send(command, ct);
    
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}