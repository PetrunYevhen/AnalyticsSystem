using FluentValidation;

namespace Analytics.Application.Commands.Users.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.CreateUserDto).NotNull().WithMessage("Дані користувача не можуть бути порожніми.");

        When(x => x.CreateUserDto != null, () =>
        {
            RuleFor(x => x.CreateUserDto.FullName)
                .NotEmpty().WithMessage("Повне ім'я не може бути порожнім.")
                .MinimumLength(2).WithMessage("Повне ім'я має містити щонайменше 2 символи.")
                .MaximumLength(200).WithMessage("Повне ім'я не може перевищувати 200 символів.");

            RuleFor(x => x.CreateUserDto.Email)
                .NotEmpty().WithMessage("Email не може бути порожнім.")
                .EmailAddress().WithMessage("Email має некоректний формат.")
                .MaximumLength(320).WithMessage("Email не може перевищувати 320 символів.");

            RuleFor(x => x.CreateUserDto.Password)
                .NotEmpty().WithMessage("Пароль не може бути порожнім.")
                .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
                .MaximumLength(128).WithMessage("Пароль не може перевищувати 128 символів.")
                .Matches("[A-Z]").WithMessage("Пароль має містити щонайменше одну велику літеру.")
                .Matches("[a-z]").WithMessage("Пароль має містити щонайменше одну малу літеру.")
                .Matches("[0-9]").WithMessage("Пароль має містити щонайменше одну цифру.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Пароль має містити щонайменше один спеціальний символ.");

            RuleFor(x => x.CreateUserDto.Role)
                .NotEmpty().WithMessage("Роль не може бути порожньою.")
                .Must(r => r is "Analyst" or "Viewer")
                .WithMessage("Роль має бути Analyst або Viewer.");
        });
    }
}