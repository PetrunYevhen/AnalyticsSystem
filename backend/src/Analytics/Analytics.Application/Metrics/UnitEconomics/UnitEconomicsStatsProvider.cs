using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Acquisition;
using Analytics.Application.Metrics.Readers.Cohort;
using Analytics.Application.Metrics.Readers.Cost;
using Analytics.Application.Metrics.Readers.Revenue;
using Analytics.Application.Metrics.Retention;
using FluentResults;

namespace Analytics.Application.Metrics.UnitEconomics;

public class UnitEconomicsStatsProvider : IUnitEconomicsStatsProvider
{
    private readonly ICostStatsReader _costStats;
    private readonly IRevenueStatsReader _revenue;
    private readonly IAcquisitionStatsReader _acquisition;
    private readonly ICohortStatsReader _cohort;
    private readonly IRetentionCalculator _retentionCalculator;
    private readonly ILtvCalculator _ltvCalculator;

    public UnitEconomicsStatsProvider(
        IRevenueStatsReader revenue,
        IAcquisitionStatsReader acquisition,
        ICohortStatsReader cohort,
        IRetentionCalculator retentionCalculator,
        ILtvCalculator ltvCalculator, ICostStatsReader costStats)
    {
        _revenue = revenue;
        _acquisition = acquisition;
        _cohort = cohort;
        _retentionCalculator = retentionCalculator;
        _ltvCalculator = ltvCalculator;
        _costStats = costStats;
    }

    public async Task<Result<UnitEconomicsStats>> GetAsync(
        Guid tenantId,
        Period period,
        CancellationToken cancellationToken)
    {
        var revenueTask = _revenue.GetAsync(tenantId, period, cancellationToken);
        var acquisitionTask = _acquisition.GetAsync(tenantId, period, cancellationToken);
        var cohortTask = _cohort.GetAsync(tenantId, period, cancellationToken);
        var costTask = _costStats.GetAsync(tenantId, period, cancellationToken);

        await Task.WhenAll(revenueTask, acquisitionTask, cohortTask, costTask);

        var revenue = revenueTask.Result;
        var acquisition = acquisitionTask.Result;
        var cohort = cohortTask.Result;
        var cost = costTask.Result;

        var grossMargin = revenue.TotalRevenue == 0m
            ? 0m
            : (revenue.TotalRevenue - cost.TotalCogs) / revenue.TotalRevenue;
        
        var retention = _retentionCalculator.Calculate(cohort, period);
        var ltv = _ltvCalculator.Calculate(revenue, retention, period, grossMargin);

        var cac = SafeDiv(acquisition.MarketingSpend, acquisition.NewCustomers);
        var ltvCac = SafeDiv(ltv.LifetimeValue, cac);
        var romi = acquisition.MarketingSpend == 0m
            ? 0m
            : (revenue.TotalRevenue * grossMargin - acquisition.MarketingSpend)
            / acquisition.MarketingSpend * 100m;

        return Result.Ok(new UnitEconomicsStats(
            ltvCac,
            ltv.LifetimeValue,
            cac,
            romi,
            ltv.LifespanMonths,
            grossMargin));
    }

    private static decimal SafeDiv(decimal a, decimal b) => b == 0m ? 0m : a / b;
    private static decimal SafeDiv(decimal a, int b) => b == 0  ? 0m : a / b;
}