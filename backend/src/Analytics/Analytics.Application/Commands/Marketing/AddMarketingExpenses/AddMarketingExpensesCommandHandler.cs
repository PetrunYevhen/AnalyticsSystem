using Analytics.Application.Auth;
using Analytics.Application.Commands.Marketing.AddMarketingExpenses.Dtos;
using Analytics.Domain.Entities.Marketing;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using ValueObjects.ValueObject;

namespace Analytics.Application.Commands.Marketing.AddMarketingExpenses;

public class AddMarketingExpensesCommandHandler : IRequestHandler<AddMarketingExpensesCommand, Result<MarketingExpenseDto>>
{
    private readonly IMarketingRepository _marketingRepository;
    private readonly ITenantContext _tenantContext;

    public AddMarketingExpensesCommandHandler(IMarketingRepository marketingRepository, ITenantContext tenantContext)
    {
        _marketingRepository = marketingRepository;
        _tenantContext = tenantContext;
    }

    public async Task<Result<MarketingExpenseDto>> Handle(AddMarketingExpensesCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();

        Currency currency;
        try
        {
            currency = Currency.Parse(request.Currency);
        }
        catch (ArgumentException ex)
        {
            return Result.Fail<MarketingExpenseDto>(ex.Message);
        }

        var totalAmount = new Money(request.Amount, currency);
        var now = DateTime.UtcNow;

        var expense = MarketingExpense.Create(
            tenantId,
            request.CampaignId,
            request.AdSource,
            totalAmount,
            request.ExpenseDate);

        if (request.Impressions.HasValue || request.Clicks.HasValue || request.Leads.HasValue)
        {
            expense.RecordMetrics(
                request.Impressions,
                request.Clicks,
                request.Leads,
                now);
        }
        else
            return Result.Fail<MarketingExpenseDto>("Метрики не вказані");


        await _marketingRepository.AddAsync(expense, cancellationToken);
        
        return Result.Ok(new MarketingExpenseDto(
            expense.Id,
            expense.AdSource,
            expense.ExpenseDate,
            expense.Amount.Amount,
            expense.Amount.Currency.Code,
            expense.CampaignId,
            expense.Impressions,
            expense.Clicks,
            expense.Leads,
            expense.ClickThroughRate,
            expense.ConversionRate));
    }

}