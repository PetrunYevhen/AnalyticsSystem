using Analytics.Application.Contracts;
using Analytics.Application.Queries.MarketingExpenses;
using Analytics.Application.Queries.MarketingExpenses.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("marketing")]
public class MarketingController : ControllerBase
{
    private readonly IAnalyticsModule _analyticsModule;

    public MarketingController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpGet]
    public async Task<ActionResult> GetMarketingExpenses()
    {
        var result = await _analyticsModule.ExecuteQueryAsync(new GetMarketingExpensesDashboardQuery());
        return Ok(result);
    }
}