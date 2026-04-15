using Analytics.Application.Queries.Dashboard;
using Analytics.Application.Queries.Dashboard.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        [FromQuery] Granularity granularity = Granularity.Monthly,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetDashboardQuery(from?.UtcDateTime, to?.UtcDateTime, granularity), ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Errors);
    }
}