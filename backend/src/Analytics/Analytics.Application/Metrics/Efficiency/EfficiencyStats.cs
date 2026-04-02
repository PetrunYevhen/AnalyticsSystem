namespace Analytics.Application.Metrics.Efficiency;

public class EfficiencyStats
{
    public EfficiencyStats(decimal totalAdSpend, decimal arpu, decimal aov, decimal repeatPurchaseRate)
    {
        TotalAdSpend = totalAdSpend;
        Arpu = arpu;
        Aov = aov;
        RepeatPurchaseRate = repeatPurchaseRate;
    }

    public decimal TotalAdSpend { get; init; }
    public decimal Arpu { get; init; }
    public decimal Aov { get; init; }
    public decimal RepeatPurchaseRate { get; init; }
}