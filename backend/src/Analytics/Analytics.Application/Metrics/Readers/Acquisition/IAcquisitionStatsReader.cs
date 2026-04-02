using Analytics.Application.Common;

namespace Analytics.Application.Metrics.Readers.Acquisition;

public interface IAcquisitionStatsReader
{
    Task<AcquisitionStats> GetAsync(
        Guid tenantId,
        Period period
        ,CancellationToken cancellationToken = default);
}