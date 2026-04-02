using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Revenue;
using Analytics.Application.Metrics.Retention;

namespace Analytics.Application.Metrics.UnitEconomics;

public interface ILtvCalculator
{
    LtvResult Calculate(
        RevenueStats revenue,
        RetentionResult retention,
        Period period,
        decimal grossMargin);

    LtvResult CalculateForCustomer(
        decimal customerTotalRevenue,
        DateOnly firstOrderDate,
        DateOnly calculationDate,
        decimal cohortMonthlyChurnRate,
        decimal grossMargin);
}