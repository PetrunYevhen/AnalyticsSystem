using Analytics.Application.Contracts;
using Analytics.Application.Queries.Orders.GetOrdersDashboard;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{ 
    private readonly IAnalyticsModule _analyticsModule;

    public OrdersController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersDashboard()
    {
        var result = await _analyticsModule.ExecuteQueryAsync(new GetOrdersDashboardQuery());
        return Ok(result);
}
}