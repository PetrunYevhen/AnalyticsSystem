using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Acquisition;
using Analytics.Application.Metrics.Readers.Revenue;
using FluentResults;

namespace Analytics.Application.Metrics.Efficiency;

public class EfficiencyStatsProvider : IEfficiencyStatsProvider
{
    private readonly IRevenueStatsReader _revenue;
    private readonly IAcquisitionStatsReader _acquisition;

    public EfficiencyStatsProvider(IRevenueStatsReader revenue, IAcquisitionStatsReader acquisition)
    {
        _revenue = revenue;
        _acquisition = acquisition;
    }

    public async Task<Result<EfficiencyStats>> GetAsync(Guid tenantId, Period period, CancellationToken ct)
    {
        var revenueTask = _revenue.GetAsync(tenantId, period, ct);
        var acquisitionTask = _acquisition.GetAsync(tenantId, period, ct);
        await Task.WhenAll(revenueTask, acquisitionTask);

        var revenue = await revenueTask;
        var acquisition = await acquisitionTask;

        var months = (decimal)(period.EndUtc - period.StartUtc).TotalDays / 30.44m;

        var aov = SafeDiv(revenue.TotalRevenue, revenue.OrdersCount);
        var arpuPeriod = SafeDiv(revenue.TotalRevenue, revenue.UniqueCustomers);
        var arpuMonthly = months <= 0 ? 0m : arpuPeriod / months;
        var repeatPurchaseRate = SafeDiv(acquisition.RepeatCustomers, acquisition.TotalCustomers) * 100;

        return Result.Ok(new EfficiencyStats(
            acquisition.MarketingSpend,
            arpuMonthly,
            aov,
            repeatPurchaseRate));
    }
    
    private static decimal SafeDiv(decimal a, int b) => b == 0 ? 0m : a / b;
}