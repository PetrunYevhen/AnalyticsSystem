namespace Analytics.Application.Metrics.UnitEconomics;

public sealed record LtvResult(
    decimal LifetimeValue,
    decimal ArpuMonthly,
    decimal LifespanMonths,
    decimal ChurnRate)
{
    public static LtvResult Empty => new(0, 0, 0, 0);
}
