using Analytics.Application.Common;

namespace Analytics.Application.Metrics.Readers.Revenue;

public interface IRevenueStatsReader
{
    Task<RevenueStats> GetAsync(
        Guid tenantId,
        Period period
        ,CancellationToken cancellationToken = default);
}