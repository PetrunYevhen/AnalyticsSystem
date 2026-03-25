using Analytics.Application.Auth;
using Analytics.Application.Commands.Auth.Login.Dtos;
using Analytics.Application.Exeptions;
using Analytics.Domain.RepositoryContracts;
using MediatR;

namespace Analytics.Application.Commands.Auth.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
{
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public LoginCommandHandler(IJwtProvider jwtProvider, IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _jwtProvider = jwtProvider;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);

        var isPasswordValid = user is not null &&
                              _passwordHasher.Verify(user.PasswordHash, request.Password);
        
        if (!isPasswordValid || user is null)
            throw new InvalidCredentialsException();

        var token = _jwtProvider.GenerateJwtToken(user);

        return new LoginResultDto(
            JwtToken: token,
            UserId: user.Id,
            TenantId: user.TenantId,
            FullName: user.FullName,
            Email: user.Email,
            Role: user.Role.Name);
    }
}