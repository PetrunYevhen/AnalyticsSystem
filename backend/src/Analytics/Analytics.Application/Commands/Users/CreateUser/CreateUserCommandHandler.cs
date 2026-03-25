using Analytics.Application.Auth;
using Analytics.Application.Commands.Users.CreateUser.Dtos;
using Analytics.Domain.Entities.User;
using Analytics.Domain.RepositoryContracts;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Analytics.Application.Commands.Users.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(IUserRepository userRepository, ITenantContext tenantContext, IPasswordHasher passwordHasher, ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tenantContext = tenantContext;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.RequiredTenantId();
    
        if (await _userRepository.ExistsByEmailAsync(request.CreateUserDto.Email, cancellationToken))
            return Result.Fail("Користувач з таким email вже існує.");

        _logger.LogInformation($"[CreateUser] Role from request: '{request.CreateUserDto.Role}'");
        if (!UserRole.TryFromName(request.CreateUserDto.Role,  out var role))
            return Result.Fail($"Невалідна роль: '{request.CreateUserDto.Role}'. Допустимі: Admin, Analyst, Viewer.");
        
        _logger.LogInformation($"[CreateUser] Final role: '{role}'");

        var passwordHash = _passwordHasher.Hash(request.CreateUserDto.Password);

        var user = User.Create(
            tenantId,
            request.CreateUserDto.FullName,
            request.CreateUserDto.Email, 
            passwordHash,
            role);

        await _userRepository.AddAsync(user);
    
        return Result.Ok(new CreateUserDto(
            user.FullName,
            user.Email,
            string.Empty, 
            user.Role.ToString()));
    }
}