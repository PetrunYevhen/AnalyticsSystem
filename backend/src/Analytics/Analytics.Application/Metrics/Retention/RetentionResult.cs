namespace Analytics.Application.Metrics.Retention;

public class RetentionResult
{
    public RetentionResult(decimal churnRate, decimal retentionRate)
    {
        ChurnRate = churnRate;
        RetentionRate = retentionRate;
    }

    public static RetentionResult Empty { get; } = new(0m, 0m);

    public decimal RetentionRate { get; init; }
    public decimal ChurnRate { get; init; }
}