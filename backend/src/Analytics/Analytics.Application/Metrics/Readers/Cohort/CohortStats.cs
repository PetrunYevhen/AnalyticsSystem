namespace Analytics.Application.Metrics.Readers.Cohort;

public class CohortStats
{
    public int CustomersAtStart { get; init;}
    public int CustomersAtEnd { get; init; } 
    public double AvgLifespanDays { get; init; }
}