using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Cohort;

namespace Analytics.Application.Metrics.Retention;

public interface IRetentionCalculator
{
    RetentionResult Calculate(CohortStats cohort, Period period);
}