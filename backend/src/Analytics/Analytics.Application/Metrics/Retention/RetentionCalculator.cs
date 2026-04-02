using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Cohort;

namespace Analytics.Application.Metrics.Retention;

public class RetentionCalculator : IRetentionCalculator
{
    public RetentionResult Calculate(CohortStats cohort, Period period)
    {
        if (cohort.CustomersAtStart <= 0)
            return new RetentionResult(0m, 0m);

        var retention = Math.Clamp(
            (decimal)cohort.CustomersAtEnd / cohort.CustomersAtStart,
            0m, 1m);

        var periodChurn = 1m - retention;

        var months = (decimal)(period.EndUtc - period.StartUtc).TotalDays / 30.44m;
        var monthlyChurn = months <= 0
            ? periodChurn : 1m - (decimal)Math.Pow((double)retention, 1.0 / (double)months);

        return new RetentionResult(retention, monthlyChurn);
    }
}