using Analytics.Application.Commands.Users.ChangePassword;
using Analytics.Application.Commands.Users.CreateUser;
using Analytics.Application.Commands.Users.CreateUser.Dtos;
using Analytics.Application.Queries.Users.GetAllUsers;
using Analytics.Application.Queries.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator) => _mediator = mediator;

    [Authorize(Policy = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var profile = await _mediator.Send(new GetProfileQuery());
        return Ok(profile);
    }

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] CreateUserDto command)
    {
        var result = await _mediator.Send(new CreateUserCommand(command));
        return Ok(result);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
    
}