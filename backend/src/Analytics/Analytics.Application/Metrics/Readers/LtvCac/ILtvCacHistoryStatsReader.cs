using Analytics.Application.Common;
using Analytics.Application.Queries.Dashboard.Enums;
using Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;

namespace Analytics.Application.Metrics.Readers.LtvCac;

public interface ILtvCacHistoryStatsReader
{
    Task<List<LtvCacDataPoint>> GetAsync(
        Guid tenantId,
        Period period,
        Granularity granularity,
        decimal lifespanMonths,
        decimal grossMargin,
        CancellationToken ct = default);
}