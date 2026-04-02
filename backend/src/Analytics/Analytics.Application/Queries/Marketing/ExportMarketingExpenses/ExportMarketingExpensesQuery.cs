using Analytics.Application.Common;
using Analytics.Application.Contracts;

namespace Analytics.Application.Queries.Marketing.ExportMarketingExpenses;

public class ExportMarketingExpensesQuery : QueryBase<byte[]>
{
    public ExportMarketingExpensesQuery(Period period)
    {
        Period = period;
    }

    public Period Period { get; set; }
}