using Analytics.Application.Contracts;
using Analytics.Application.Queries.Transactions.GetTransactiosDashboard;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IAnalyticsModule _analyticsModule;

    public TransactionsController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpGet]
    public async Task<IActionResult> GetTransactionsDashboard()
    {
        var result = await _analyticsModule.ExecuteQueryAsync(new GetTransactionsDashboardQuery());
        return Ok(result);
    }
}