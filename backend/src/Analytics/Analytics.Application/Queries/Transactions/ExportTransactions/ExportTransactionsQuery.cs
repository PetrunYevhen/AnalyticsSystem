using Analytics.Application.Common;
using Analytics.Application.Contracts;

namespace Analytics.Application.Queries.Transactions.ExportTransactions;

public class ExportTransactionsQuery : QueryBase<byte[]>
{
    public ExportTransactionsQuery(Period period)
    {
        Period = period;
    }

    public Period Period { get; set; }
}