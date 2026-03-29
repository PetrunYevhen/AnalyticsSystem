using Analytics.Application.Common;
using Analytics.Application.Contracts;

namespace Analytics.Application.Queries.Orders.ExportOrders;

public class ExportOrdersQuery : QueryBase<byte[]>
{
    public ExportOrdersQuery(Period period)
    {
        Period = period;
    }

    public Period Period { get; set; }
}