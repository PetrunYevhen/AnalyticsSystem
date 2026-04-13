using Analytics.Application.Commands.Tenant.CreateTenant;
using Analytics.Application.Contracts;
using Analytics.Application.Queries.Tenants.GetTenant;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("tenants")]
public class TenantsController : ControllerBase 
{
    private readonly IAnalyticsModule _analyticsModule;

    public TenantsController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTenant(Guid id)
    {
        var result = await _analyticsModule.ExecuteQueryAsync(new GetTenantQuery(id));
        
        if (result == null)
        {
            return NotFound(); 
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantCommand command)
    {
        var result = await _analyticsModule.ExecuteCommandAsync(command);
        
        return CreatedAtAction(nameof(GetTenant), new { id = result }, result);
    }
}