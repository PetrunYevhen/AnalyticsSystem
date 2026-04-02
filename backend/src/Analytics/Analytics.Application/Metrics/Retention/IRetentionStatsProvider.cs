using Analytics.Application.Common;
using FluentResults;

namespace Analytics.Application.Metrics.Retention;

public interface IRetentionStatsProvider
{
    Task<Result<RetentionStats>> GetAsync(Guid tenantId, Period period, CancellationToken cancellationToken = default);
}