using Analytics.Application.Auth;
using Analytics.Application.Common;
using Analytics.Application.Metrics.Efficiency;
using Analytics.Application.Metrics.Readers.LtvCac;
using Analytics.Application.Metrics.Retention;
using Analytics.Application.Metrics.UnitEconomics;
using Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Queries.Marketing.GetMarketingDashboard;

public sealed class GetMarketingDashboardQueryHandler
    : IRequestHandler<GetMarketingDashboardQuery, Result<MarketingDashboardDto>>
{
    private readonly IUnitEconomicsStatsProvider _unitEconomics;
    private readonly IEfficiencyStatsProvider _efficiency;
    private readonly IRetentionStatsProvider _retention;
    private readonly ILtvCacHistoryStatsReader _ltvCacHistoryStats;
    private readonly ITenantContext _tenant;
    private readonly ILogger<GetMarketingDashboardQueryHandler> _logger;

    public GetMarketingDashboardQueryHandler(
        IEfficiencyStatsProvider efficiency,
        ITenantContext tenant,
        IUnitEconomicsStatsProvider unitEconomicsStatsProvider,
        IRetentionStatsProvider retention,
        ILtvCacHistoryStatsReader ltvCacHistoryStats, ILogger<GetMarketingDashboardQueryHandler> logger)
    {
        _efficiency = efficiency;
        _tenant = tenant;
        _unitEconomics = unitEconomicsStatsProvider;
        _retention = retention;
        _ltvCacHistoryStats = ltvCacHistoryStats;
        _logger = logger;
    }

    public async Task<Result<MarketingDashboardDto>> Handle(
        GetMarketingDashboardQuery request, CancellationToken ct)
    {
        var tenantId = _tenant.RequiredTenantId();
        
        var periodResult = Period.ResolvePeriod(request.FromDate, request.ToDate);
        if (periodResult.IsFailed)
            return Result.Fail<MarketingDashboardDto>(periodResult.Errors);

        var period = periodResult.Value;
        
        var uEconTask = SafeCall(() => _unitEconomics.GetAsync(tenantId, period, ct), nameof(_unitEconomics));
        var effTask = SafeCall(() => _efficiency.GetAsync(tenantId, period, ct), nameof(_efficiency));
        var retTask = SafeCall(() => _retention.GetAsync(tenantId, period, ct), nameof(_retention));
        var historyTask = FetchHistory();

        await Task.WhenAll(uEconTask, effTask, retTask, historyTask);

        async Task<Result<List<LtvCacDataPoint>>> FetchHistory()
        {
            var uEcon= await uEconTask; 
            var lifespanMonths = uEcon.IsSuccess ? uEcon.Value.LifespanMonths : 0m;
            var grossMargin = uEcon.IsSuccess ? uEcon.Value.GrossMargin : 0m;
            return await WrapNonResult(
                _ltvCacHistoryStats.GetAsync(tenantId, period, request.Granularity, lifespanMonths, grossMargin, ct));
        }
        
        var merged = Result.Merge(uEconTask.Result, effTask.Result, retTask.Result, historyTask.Result);
        if (merged.IsFailed)
            return merged.ToResult<MarketingDashboardDto>();

        return Result.Ok(new MarketingDashboardDto(
            uEconTask.Result.Value,
            effTask.Result.Value,
            retTask.Result.Value,
            historyTask.Result.Value));
    }

    private async Task<Result<T>> SafeCall<T>(Func<Task<Result<T>>> call, string source)
    {
        try
        {
            return await call();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dependency {Source} failed", source);
            return Result.Fail<T>(new Error($"Dependency {source} failed.").CausedBy(ex));
        }
    }

    private static async Task<Result<T>> WrapNonResult<T>(Task<T> task)
    {
        var value = await task;
        return Result.Ok(value);
    }
}