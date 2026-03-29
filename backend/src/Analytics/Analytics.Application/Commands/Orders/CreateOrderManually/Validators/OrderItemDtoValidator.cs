using Analytics.Application.Commands.Orders.CreateOrderManually.Dto;
using FluentValidation;

namespace Analytics.Application.Commands.Orders.CreateOrderManually.Validators;

public sealed class OrderItemDtoValidator : AbstractValidator<OrderItemDto>
{
    public OrderItemDtoValidator()
    {
        RuleFor(x => x.ProductExternalId)
            .MaximumLength(100).WithMessage("Зовнішній ідентифікатор продукту не може перевищувати 100 символів.")
            .When(x => x.ProductExternalId is not null);

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Назва продукту не може бути порожньою.")
            .MaximumLength(300).WithMessage("Назва продукту не може перевищувати 300 символів.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Категорія не може бути порожньою.")
            .MaximumLength(100).WithMessage("Категорія не може перевищувати 100 символів.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Кількість має бути більшою за нуль.")
            .LessThanOrEqualTo(100_000).WithMessage("Кількість не може перевищувати 100 000 одиниць.");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Собівартість одиниці не може бути від'ємною.")
            .PrecisionScale(18, 4, false)
            .WithMessage("Собівартість не може мати більше 4 знаків після коми.");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Ціна одиниці має бути більшою за нуль.")
            .PrecisionScale(18, 4, false)
            .WithMessage("Ціна не може мати більше 4 знаків після коми.");

        RuleFor(x => x)
            .Must(i => i.UnitPrice >= i.UnitCost)
            .WithMessage("Ціна продажу не може бути меншою за собівартість.")
            .OverridePropertyName(nameof(OrderItemDto.UnitPrice));
    }
}