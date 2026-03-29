using Analytics.Application.Contracts;
using Analytics.Application.Queries.Customers.GetCustomer.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Customers.GetCustomer;

public class GetCustomerDetailsQuery : QueryBase<Result<CustomerDetailsDto>>
{
    public GetCustomerDetailsQuery(Guid customerId)
    {
        CustomerId = customerId;
    }

    public Guid CustomerId { get; set; }
}