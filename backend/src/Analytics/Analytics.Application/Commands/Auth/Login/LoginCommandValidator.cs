using FluentValidation;

namespace Analytics.Application.Commands.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email не може бути порожнім.")
            .EmailAddress().WithMessage("Email має некоректний формат.")
            .MaximumLength(320).WithMessage("Email не може перевищувати 320 символів.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль не може бути порожнім.")
            .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
            .MaximumLength(128).WithMessage("Пароль не може перевищувати 128 символів.");
    }
}