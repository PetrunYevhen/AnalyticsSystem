using Analytics.Application.Common;

namespace Analytics.Application.Metrics.Readers.Cohort;

public interface ICohortStatsReader
{
    Task<CohortStats> GetAsync(
        Guid tenantId,
        Period period
        ,CancellationToken cancellationToken = default);
}