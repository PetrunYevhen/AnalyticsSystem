using Analytics.Application.Common;
using Analytics.Application.Metrics.Readers.Revenue;
using Analytics.Application.Metrics.Retention;

namespace Analytics.Application.Metrics.UnitEconomics;

public class LtvCalculator : ILtvCalculator
{
    private const decimal MaxLifespanMonths = 60m;
    private const decimal MinChurn = 0.005m;
    private const decimal DaysPerMonth = 30.44m;

    public LtvResult Calculate(
        RevenueStats revenue,
        RetentionResult retention,
        Period period,
        decimal grossMargin)
    {
        if (revenue.UniqueCustomers <= 0) return LtvResult.Empty;

        var months = (decimal)(period.EndUtc - period.StartUtc).TotalDays / DaysPerMonth;
        if (months <= 0) return LtvResult.Empty;

        var arpuMonthly = revenue.TotalRevenue / revenue.UniqueCustomers / months;

        var churn = Math.Max(retention.ChurnRate, MinChurn);
        var lifespan = Math.Min(1m / churn, MaxLifespanMonths);

        var ltv = arpuMonthly * grossMargin * lifespan;

        return new LtvResult(ltv, arpuMonthly, lifespan, churn);
    }

    public LtvResult CalculateForCustomer(decimal customerTotalRevenue, DateOnly firstOrderDate, DateOnly asOfDate,
        decimal cohortMonthlyChurnRate, decimal grossMargin)
    {
        if (customerTotalRevenue <= 0) return LtvResult.Empty;

        var lifespanMonths = (asOfDate.DayNumber - firstOrderDate.DayNumber) / DaysPerMonth;
        if (lifespanMonths <= 0) return LtvResult.Empty;

        var arpuMonthly = customerTotalRevenue / lifespanMonths;
        var churn = Math.Max(cohortMonthlyChurnRate, MinChurn);
        var lifespan = Math.Min(1m / churn, MaxLifespanMonths);
        var ltv = arpuMonthly * grossMargin * lifespan;
        return new LtvResult(ltv, arpuMonthly, lifespan, churn);
    }
}