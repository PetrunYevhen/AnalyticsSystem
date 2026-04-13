using Application;
using Dapper;
using MediatR;

namespace Analytics.Application.Queries.Customer.GetCustomers;

public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, List<CustomerDto>>
{
    private readonly INpgsqlConnectionFactory _connectionFactory ;

    public GetAllCustomersQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();

        const string sql = @"
        SELECT 
            ""Id"", 
            ""FullName"", 
            ""Email"", 
            ""Status"", 
            ""LastOrderDate"", 
            ""AcquisitionChannel""
        FROM ""Analytics"".""Customers""
        WHERE ""FirstOrderDate"" IS NOT NULL 
        ORDER BY ""LastOrderDate"" DESC";

        var customer = await connection.QueryAsync<CustomerDto>(sql);
        return customer.ToList();
    }
}