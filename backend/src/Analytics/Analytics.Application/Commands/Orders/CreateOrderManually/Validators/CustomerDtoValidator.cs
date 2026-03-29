using Analytics.Application.Commands.Orders.CreateOrderManually.Dto;
using FluentValidation;

namespace Analytics.Application.Commands.Orders.CreateOrderManually.Validators;

public sealed class CustomerDtoValidator : AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім.")
            .EmailAddress().WithMessage("Email має некоректний формат.")
            .MaximumLength(320).WithMessage("Email не може перевищувати 320 символів.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Повне ім'я не може бути порожнім.")
            .MinimumLength(2).WithMessage("Повне ім'я має містити щонайменше 2 символи.")
            .MaximumLength(200).WithMessage("Повне ім'я не може перевищувати 200 символів.")
            .When(x => x.IsNewCustomer);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Номер телефону не може бути порожнім.")
            .Matches(@"^\+?[0-9\s\-\(\)]{7,20}$")
            .WithMessage("Номер телефону має некоректний формат.")
            .MaximumLength(20).WithMessage("Номер телефону не може перевищувати 20 символів.")
            .When(x => x.IsNewCustomer);

        RuleFor(x => x.ExternalId)
            .MaximumLength(100).WithMessage("Зовнішній ідентифікатор не може перевищувати 100 символів.")
            .When(x => x.ExternalId is not null);

        RuleFor(x => x.AcquisitionChannel)
            .IsInEnum().WithMessage("Вказаний канал залучення не існує в системі.")
            .When(x => x.IsNewCustomer);

        RuleFor(x => x.RegistrationDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Дата реєстрації не може бути у майбутньому.")
            .When(x => x.RegistrationDate.HasValue);
    }
}