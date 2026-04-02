namespace Analytics.Application.Metrics.UnitEconomics;

public sealed record UnitEconomicsStats(
    decimal LtvToCac,
    decimal Ltv,
    decimal Cac,
    decimal Romi,
    decimal LifespanMonths,
    decimal GrossMargin);
