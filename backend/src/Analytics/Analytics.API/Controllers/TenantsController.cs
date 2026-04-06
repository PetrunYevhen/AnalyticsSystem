using Analytics.Application.Commands.CreateTenant;
using Analytics.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("tenants")]
public class TenantsController : Controller
{
    private readonly IAnalyticsModule _analyticsModule;

    public TenantsController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateTenantCommand command)
    {
        var result = await _analyticsModule.ExecuteCommandAsync(command);
        return Ok(result);
    }
}