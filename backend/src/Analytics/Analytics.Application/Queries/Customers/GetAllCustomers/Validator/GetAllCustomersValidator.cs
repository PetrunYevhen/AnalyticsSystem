using FluentValidation;

namespace Analytics.Application.Queries.Customers.GetAllCustomers.Validator;

public sealed class GetAllCustomersQueryValidator : AbstractValidator<GetAllCustomersQuery>
{
    private const int MaxPageSize = 200;

    public GetAllCustomersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Номер сторінки має бути більшим або рівним 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"Розмір сторінки має бути від 1 до {MaxPageSize}.");

        RuleFor(x => x.SortBy)
            .IsInEnum().WithMessage($"Невалідне поле сортування.");

        RuleFor(x => x.Direction)
            .IsInEnum().WithMessage("Невалідний напрямок сортування.");

        RuleFor(x => x.Search)
            .MaximumLength(200).WithMessage("Пошуковий запит не може перевищувати 200 символів.")
            .When(x => x.Search is not null);
    }
}