using Analytics.Application.Commands.Marketing.AddMarketingExpenses.Dtos;
using FluentValidation;

namespace Analytics.Application.Commands.Marketing.AddMarketingExpenses;

public sealed class MarketingExpenseDtoValidator : AbstractValidator<MarketingExpenseDto>
{
    private static readonly string[] AllowedCurrencies = ["UAH"];

    public MarketingExpenseDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ідентифікатор витрати не може бути порожнім.");

        RuleFor(x => x.AdSource)
            .NotEmpty().WithMessage("Джерело реклами не може бути порожнім.")
            .MaximumLength(100).WithMessage("Джерело реклами не може перевищувати 100 символів.");

        RuleFor(x => x.ExpenseDate)
            .NotEmpty().WithMessage("Дата витрати не може бути порожньою.")
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("Дата витрати не може бути у майбутньому.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Сума витрати має бути більшою за нуль.")
            .PrecisionScale(18, 4, false)
                .WithMessage("Сума витрати не може мати більше 4 знаків після коми.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта не може бути порожньою.")
            .Length(3).WithMessage("Код валюти має складатися з 3 символів (ISO 4217).")
            .Must(c => AllowedCurrencies.Contains(c.ToUpperInvariant()))
                .WithMessage($"Валюта має бути однією з: {string.Join(", ", AllowedCurrencies)}.");

        RuleFor(x => x.Impressions)
            .GreaterThanOrEqualTo(0).WithMessage("Кількість показів не може бути від'ємною.")
            .When(x => x.Impressions.HasValue);

        RuleFor(x => x.Clicks)
            .GreaterThanOrEqualTo(0).WithMessage("Кількість кліків не може бути від'ємною.")
            .When(x => x.Clicks.HasValue);

        RuleFor(x => x.Leads)
            .GreaterThanOrEqualTo(0).WithMessage("Кількість лідів не може бути від'ємною.")
            .When(x => x.Leads.HasValue);

        RuleFor(x => x.Clicks)
            .LessThanOrEqualTo(x => x.Impressions)
                .WithMessage("Кількість кліків не може перевищувати кількість показів.")
            .When(x => x.Clicks.HasValue && x.Impressions.HasValue);

        RuleFor(x => x.Leads)
            .LessThanOrEqualTo(x => x.Clicks)
                .WithMessage("Кількість лідів не може перевищувати кількість кліків.")
            .When(x => x.Leads.HasValue && x.Clicks.HasValue);

        RuleFor(x => x.ClickThroughRate)
            .InclusiveBetween(0.0, 1.0)
                .WithMessage("CTR має бути у діапазоні від 0 до 1 (0% – 100%).")
            .When(x => x.ClickThroughRate.HasValue);

        RuleFor(x => x.ConversionRate)
            .InclusiveBetween(0.0, 1.0)
                .WithMessage("Коефіцієнт конверсії має бути у діапазоні від 0 до 1 (0% – 100%).")
            .When(x => x.ConversionRate.HasValue);
    }
}