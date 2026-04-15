using Analytics.Application.Queries.Tenants.GetTenant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[Authorize]
[ApiController]
[Route("api/tenant")]
public class TenantController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("info")]
    public async Task<IActionResult> GetTenantInfo(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTenantQuery(), ct);
        return Ok(result.Value);
    }
    
    
}