using Analytics.Application.Contracts;
using Analytics.Infrastructure.SeedMockData;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SeedMockDataController : ControllerBase
{
    private readonly IAnalyticsModule _analyticsModule;

    public SeedMockDataController(IAnalyticsModule analyticsModule)
    {
        _analyticsModule = analyticsModule;
    }

    [HttpPost("seed-ecommerce-data")]
    public async Task<IActionResult> SeedData([FromQuery] int customerCount = 100)
    {
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var command = new SeedMockDataCommand
        {
            TenantId = tenantId,
            CustomerCount = customerCount
        };

        try
        {
            var result = await _analyticsModule.ExecuteCommandAsync(command);

            return Ok(new
            {
                Message = "Базу даних успішно наповнено!",
                CustomersAdded = customerCount
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = "Помилка при збереженні в базу",
                Details = ex.Message,
                InnerException = ex.InnerException?.Message
            });
        }
    }
}