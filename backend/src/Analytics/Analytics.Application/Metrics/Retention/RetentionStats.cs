namespace Analytics.Application.Metrics.Retention;

public class RetentionStats
{
    public RetentionStats(decimal retentionRate, decimal churnRate, decimal purchaseFrequency, int newCustomers, int returningCustomers)
    {
        RetentionRate = retentionRate;
        ChurnRate = churnRate;
        PurchaseFrequency = purchaseFrequency;
        NewCustomers = newCustomers;
        ReturningCustomers = returningCustomers;
    }

    public decimal RetentionRate { get; init; }
    public decimal ChurnRate { get; init; }
    public decimal PurchaseFrequency { get; init; }
    public int NewCustomers { get; init; }
    public int ReturningCustomers { get; init; }
}