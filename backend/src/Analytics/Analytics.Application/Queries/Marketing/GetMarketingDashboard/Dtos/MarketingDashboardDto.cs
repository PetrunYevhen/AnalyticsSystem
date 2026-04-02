using Analytics.Application.Metrics.Efficiency;
using Analytics.Application.Metrics.Retention;
using Analytics.Application.Metrics.UnitEconomics;

namespace Analytics.Application.Queries.Marketing.GetMarketingDashboard.Dtos;

public class MarketingDashboardDto
{
    public MarketingDashboardDto(
        UnitEconomicsStats unitEconomics,
        EfficiencyStats efficiency,
        RetentionStats retention,
        List<LtvCacDataPoint> ltvCacHistory)
    {
        UnitEconomics  = unitEconomics;
        Efficiency     = efficiency;
        Retention      = retention;
        LtvCacHistory  = ltvCacHistory;
    }

    public UnitEconomicsStats    UnitEconomics  { get; init; }
    public EfficiencyStats       Efficiency     { get; init; }
    public RetentionStats        Retention      { get; init; }
    public List<LtvCacDataPoint> LtvCacHistory  { get; init; }
}