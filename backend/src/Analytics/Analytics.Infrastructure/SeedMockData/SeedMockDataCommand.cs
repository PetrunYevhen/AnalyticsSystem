using Analytics.Application.Contracts;

namespace Analytics.Infrastructure.SeedMockData;

public class SeedMockDataCommand : CommandBase<bool>
{
    public Guid TenantId { get; set; }
    public int CustomerCount { get; set; }
}