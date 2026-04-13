using Application;
using MediatR;

namespace Analytics.Application.Queries.MarketingExpenses;

public class GetMarketingExpensesQueryHandler : IRequestHandler<GetMarketingExpensesDashboardQuery, MarketingExpensesDto>
{
    private readonly INpgsqlConnectionFactory _connectionFactory;

    public GetMarketingExpensesQueryHandler(INpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<MarketingExpensesDto> Handle(GetMarketingExpensesDashboardQuery request, CancellationToken cancellationToken)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        throw new NotImplementedException();
    }
}