using Analytics.Application.Common;
using FluentResults;

namespace Analytics.Application.Metrics.Efficiency;

public interface IEfficiencyStatsProvider
{
    Task<Result<EfficiencyStats>> GetAsync(Guid tenantId, Period period, CancellationToken ct);
}