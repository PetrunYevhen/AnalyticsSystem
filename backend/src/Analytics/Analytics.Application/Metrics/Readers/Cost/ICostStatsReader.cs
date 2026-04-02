using Analytics.Application.Common;

namespace Analytics.Application.Metrics.Readers.Cost;

public interface ICostStatsReader
{
    Task<CostStats> GetAsync(Guid tenantId, Period period, CancellationToken ct = default);

}