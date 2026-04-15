using Analytics.Application.Commands.Marketing.AddCampaignSpend;
using Analytics.Application.Commands.Marketing.AddMarketingExpenses;
using Analytics.Application.Commands.Marketing.AssignToCampaign;
using Analytics.Application.Commands.Marketing.CreateCampaign;
using Analytics.Application.Commands.Marketing.DeleteFromCampaign;
using Analytics.Application.Commands.Marketing.ImportMarketingExpenses;
using Analytics.Application.Commands.Marketing.UpdateCampaign;
using Analytics.Application.Common;
using Analytics.Application.Queries.Dashboard.Enums;
using Analytics.Application.Queries.Marketing.ExportMarketingExpenses;
using Analytics.Application.Queries.Marketing.GetAllCampaigns;
using Analytics.Application.Queries.Marketing.GetCampaignCustomers;
using Analytics.Application.Queries.Marketing.GetCustomersWithoutCampaign;
using Analytics.Application.Queries.Marketing.GetMarketingDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/marketing")]
[Authorize]
public class MarketingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MarketingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> GetMarketingDashboard(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate,
        [FromQuery] Granularity granularity = Granularity.Monthly,
        CancellationToken ct = default)
    {
        var query = new GetMarketingDashboardQuery
            (fromDate, 
                toDate, 
                granularity);
        var result = await _mediator.Send(query, ct);
        if (result.IsFailed) return BadRequest(new { errors = result.Errors.Select(e => e.Message) });
        return Ok(result.Value);
    }

    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost]
    public async Task<IActionResult> AddMarketingExpenses([FromBody] AddMarketingExpensesCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("campaigns")]
    public async Task<IActionResult> GetCampaigns()
    {
        var result = await _mediator.Send(new GetAllCampaignsQuery());
        if (result.IsFailed)
            return BadRequest(result.Errors);
        return Ok(result.Value);
    }

    [HttpGet("campaigns/{campaignId}/customers")]
    public async Task<IActionResult> GetCampaignCustomers(Guid campaignId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCampaignCustomersQuery(campaignId), ct);
        if (result.IsFailed)
            return BadRequest(result.Errors);
        
        return Ok(result.Value);
    }

    [HttpGet("campaigns/available-customers")]
    public async Task<IActionResult> GetCampaignAvailableCustomers(CancellationToken ct)
    {
        var result = await _mediator.Send(
            new GetCustomersWithoutCampaignQuery(), ct);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
    
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        var periodResult = Period.ResolvePeriod(from, to);
        if (periodResult.IsFailed)
            return BadRequest(periodResult.Errors);

        var query = new ExportMarketingExpensesQuery(periodResult.Value);
        var bytes = await _mediator.Send(query);

        return File(bytes, "text/csv", $"marketing-expenses_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
    }
    

    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("campaigns/{id}/customers")]
    public async Task<IActionResult> AssignCampaignToCustomer(
        [FromRoute] Guid id,
        [FromBody] CustomersRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new AssignCampaignToCustomersCommand
        {
            CampaignId  = id,
            CustomerIds = request.CustomerIds
        }, ct);

        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
    
    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPost("campaigns")]
    public async Task<IActionResult> CreateCampaign(
        [FromBody] CreateCampaignCommand command,
        CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }

    [HttpDelete("campaigns/{id}/customers")]
    public async Task<IActionResult> DeleteCustomers(
        [FromRoute] Guid id,
        [FromBody] CustomersRequest request,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteCustomersFromCampaignCommand {
            CampaignId  = id,
            CustomerIds = request.CustomerIds
        }, ct);
        
        if (result.IsFailed)
            return BadRequest(result.Errors);

        return NoContent();
    }

    [Authorize(Policy = "AnalystOrAbove")]
    [HttpPatch("campaigns/{id}")]
    public async Task<IActionResult> PatchCampaign(
        [FromRoute] Guid id,
        [FromBody] UpdateCampaignCommand command,
        CancellationToken ct)
    {
        command.CampaignId = id;
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Errors);
    }
    
    [HttpPost("{campaignId:guid}/spend")]
    public async Task<IActionResult> AddSpend(
        Guid campaignId,
        [FromBody] AddCampaignSpendCommand command,
        CancellationToken ct)
    {
        command.CampaignId = campaignId;
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Errors);
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
    
        var command = new ImportMarketingExpensesCommand(file);
        var result = await _mediator.Send(command, ct);
    
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
    
    public record CustomersRequest(List<Guid> CustomerIds);

}