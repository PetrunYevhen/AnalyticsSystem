using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Acquisition;
using Analytics.Application.Metrics.Readers.Cohort;
using Analytics.Application.Metrics.Readers.Revenue;
using FluentResults;

namespace Analytics.Application.Metrics.Retention;

public class RetentionStatsProvider : IRetentionStatsProvider
{
    private readonly IRevenueStatsReader _revenue;
    private readonly IAcquisitionStatsReader _acquisition;
    private readonly ICohortStatsReader _cohort;
    private readonly IRetentionCalculator _retentionCalculator;

    public RetentionStatsProvider(
        IRevenueStatsReader revenue,
        IAcquisitionStatsReader acquisition,
        ICohortStatsReader cohort,
        IRetentionCalculator retentionCalculator)
    {
        _revenue = revenue;
        _acquisition = acquisition;
        _cohort = cohort;
        _retentionCalculator = retentionCalculator;
    }

    public async Task<Result<RetentionStats>> GetAsync(
        Guid tenantId,
        Period period,
        CancellationToken cancellationToken = default)
    {
        var revenueTask = _revenue.GetAsync(tenantId, period, cancellationToken);
        var acquisitionTask = _acquisition.GetAsync(tenantId, period, cancellationToken);
        var cohortTask = _cohort.GetAsync(tenantId, period, cancellationToken);

        await Task.WhenAll(revenueTask, acquisitionTask, cohortTask);

        var revenue = revenueTask.Result;
        var acquisition = acquisitionTask.Result;
        var cohort = cohortTask.Result;

        var retention = _retentionCalculator.Calculate(cohort, period);

        var purchaseFrequency  = SafeDiv(revenue.OrdersCount, revenue.UniqueCustomers);
        var returningCustomers = Math.Max(0, revenue.UniqueCustomers - acquisition.NewCustomers);

        var result = new RetentionStats(
            retention.RetentionRate,
            retention.ChurnRate,
            purchaseFrequency,
            acquisition.NewCustomers,
            returningCustomers);

        return Result.Ok(result);
    }

    private static decimal SafeDiv(int a, int b) => b == 0 ? 0m : (decimal)a / b;
}