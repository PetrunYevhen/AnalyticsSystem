using Analytics.Application.Common;
using FluentResults;

namespace Analytics.Application.Metrics.UnitEconomics;

public interface IUnitEconomicsStatsProvider
{
    Task<Result<UnitEconomicsStats>> GetAsync(Guid tenantId, Period period, CancellationToken cancellationToken = default);
}