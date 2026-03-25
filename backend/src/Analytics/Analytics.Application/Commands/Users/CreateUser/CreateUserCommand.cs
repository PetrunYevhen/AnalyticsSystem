using Analytics.Application.Commands.Users.CreateUser.Dtos;
using Analytics.Application.Contracts;
using FluentResults;

namespace Analytics.Application.Commands.Users.CreateUser;

public class CreateUserCommand : CommandBase<Result<CreateUserDto>>
{
    public CreateUserCommand(CreateUserDto? createUserDto)
    {
        CreateUserDto = createUserDto;
    }

    public CreateUserDto? CreateUserDto { get; set; }
}