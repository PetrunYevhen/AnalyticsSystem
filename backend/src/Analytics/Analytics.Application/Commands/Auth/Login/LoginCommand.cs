using Analytics.Application.Commands.Auth.Login.Dtos;
using Analytics.Application.Contracts;

namespace Analytics.Application.Commands.Auth.Login;

public class LoginCommand : CommandBase<LoginResultDto>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}