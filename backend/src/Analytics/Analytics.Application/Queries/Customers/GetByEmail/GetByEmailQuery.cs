using Analytics.Application.Contracts;
using Analytics.Application.Queries.Customers.GetByEmail.Dtos;
using FluentResults;

namespace Analytics.Application.Queries.Customers.GetByEmail;

public class GetByEmailQuery : QueryBase<Result<CustomerDto>>
{
    public GetByEmailQuery(string email)
    {
        Email = email;
    }

    public string Email { get; set; }
}