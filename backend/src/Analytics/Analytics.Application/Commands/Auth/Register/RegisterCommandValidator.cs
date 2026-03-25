using FluentValidation;

namespace Analytics.Application.Commands.Auth.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Назва компанії не може бути порожньою.")
            .MinimumLength(2).WithMessage("Назва компанії має містити щонайменше 2 символи.")
            .MaximumLength(200).WithMessage("Назва компанії не може перевищувати 200 символів.");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Повне ім'я не може бути порожнім.")
            .MinimumLength(2).WithMessage("Повне ім'я має містити щонайменше 2 символи.")
            .MaximumLength(200).WithMessage("Повне ім'я не може перевищувати 200 символів.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім.")
            .EmailAddress().WithMessage("Email має некоректний формат.")
            .MaximumLength(320).WithMessage("Email не може перевищувати 320 символів.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім.")
            .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
            .MaximumLength(128).WithMessage("Пароль не може перевищувати 128 символів.")
            .Matches("[A-Z]").WithMessage("Пароль має містити щонайменше одну велику літеру.")
            .Matches("[a-z]").WithMessage("Пароль має містити щонайменше одну малу літеру.")
            .Matches("[0-9]").WithMessage("Пароль має містити щонайменше одну цифру.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Пароль має містити щонайменше один спеціальний символ.");
    }
}