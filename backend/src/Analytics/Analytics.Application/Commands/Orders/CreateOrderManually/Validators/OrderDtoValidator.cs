using Analytics.Application.Commands.Orders.CreateOrderManually.Dto;
using FluentValidation;

namespace Analytics.Application.Commands.Orders.CreateOrderManually.Validators;

public sealed class OrderDtoValidator : AbstractValidator<OrderDto>
{
    private static readonly string[] AllowedCurrencies = ["UAH"];
    private static readonly string[] AllowedStatuses   = ["Pending", "Processing", "Completed", "Cancelled"];
    private static readonly string[] AllowedMethods    = ["Cash", "Card", "BankTransfer"];

    public OrderDtoValidator()
    {
        RuleFor(x => x.ExternalOrderId)
            .MaximumLength(100).WithMessage("Зовнішній ідентифікатор замовлення не може перевищувати 100 символів.")
            .When(x => x.ExternalOrderId is not null);

        RuleFor(x => x.OrderDate)
            .NotEmpty().WithMessage("Дата замовлення не може бути порожньою.")
            .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("Дата замовлення не може бути у майбутньому.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Статус замовлення не може бути порожнім.")
            .Must(s => AllowedStatuses.Contains(s))
                .WithMessage($"Статус має бути одним з: {string.Join(", ", AllowedStatuses)}.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Валюта не може бути порожньою.")
            .Length(3).WithMessage("Код валюти має складатися з 3 символів (ISO 4217).")
            .Must(c => AllowedCurrencies.Contains(c.ToUpperInvariant()))
                .WithMessage($"Валюта має бути однією з: {string.Join(", ", AllowedCurrencies)}.");

        RuleFor(x => x.InitialPaymentAmount)
            .GreaterThan(0).WithMessage("Сума початкової оплати має бути більшою за нуль.")
            .PrecisionScale(18, 4, false)
                .WithMessage("Сума початкової оплати не може мати більше 4 знаків після коми.")
            .When(x => x.InitialPaymentAmount.HasValue);

        RuleFor(x => x.InitialPaymentMethod)
            .Must(m => AllowedMethods.Contains(m))
                .WithMessage($"Метод оплати має бути одним з: {string.Join(", ", AllowedMethods)}.")
            .When(x => x.InitialPaymentMethod is not null);

        RuleFor(x => x.InitialPaymentNote)
            .MaximumLength(500).WithMessage("Примітка до оплати не може перевищувати 500 символів.")
            .When(x => x.InitialPaymentNote is not null);

        RuleFor(x => x.Items)
            .NotNull().WithMessage("Список позицій замовлення не може бути відсутнім.")
            .NotEmpty().WithMessage("Замовлення має містити щонайменше одну позицію.");

        RuleForEach(x => x.Items)
            .SetValidator(new OrderItemDtoValidator());

        RuleFor(x => x)
            .Must(o => o.CustomerId.HasValue || true)
            .Must(HaveConsistentPaymentFields)
                .WithMessage("Метод оплати є обов'язковим якщо вказана сума початкової оплати.");
    }

    private static bool HaveConsistentPaymentFields(OrderDto o) =>
        !o.InitialPaymentAmount.HasValue || o.InitialPaymentMethod is not null;
}