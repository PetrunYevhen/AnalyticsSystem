namespace Analytics.Application.Metrics.Readers.Acquisition;

public sealed class AcquisitionStats
{
    public int NewCustomers {get; init;}
    public decimal MarketingSpend {get; init;}
    public int RepeatCustomers { get; init; }
    public int TotalCustomers { get; init; }
    
}