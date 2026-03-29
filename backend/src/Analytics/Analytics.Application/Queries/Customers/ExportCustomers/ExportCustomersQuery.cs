using Analytics.Application.Common;
using Analytics.Application.Contracts;

namespace Analytics.Application.Queries.Customers.ExportCustomers;

public class ExportCustomersQuery : QueryBase<byte[]>
{
    public ExportCustomersQuery(Period period)
    {
        Period = period;
    }

    public Period Period { get; set; }
}

