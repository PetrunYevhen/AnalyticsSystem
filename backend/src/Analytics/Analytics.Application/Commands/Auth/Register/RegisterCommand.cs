using Analytics.Application.Commands.Auth.Register.Dtos;
using Analytics.Application.Contracts;

namespace Analytics.Application.Commands.Auth.Register;

public class RegisterCommand : CommandBase<RegisterResultDto>
{
    public required string CompanyName { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}
