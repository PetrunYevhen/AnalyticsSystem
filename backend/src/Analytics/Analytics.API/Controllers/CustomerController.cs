using Analytics.Application.Contracts;
using Analytics.Application.Queries.Customers.GetCustomers;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("customer")]
public class CustomerController : ControllerBase
{
    private readonly IAnalyticsModule _analyticsModule;

    public CustomerController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var result = await _analyticsModule.ExecuteQueryAsync(new GetAllCustomersQuery());
        return Ok(result);
    }
}