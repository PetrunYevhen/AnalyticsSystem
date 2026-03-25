using Analytics.Application.Auth;
using Analytics.Application.Auth.ApiKey;
using Analytics.Application.Commands.Auth.Register.Dtos;
using Analytics.Application.Exeptions;
using Analytics.Domain.Entities.User;
using Analytics.Domain.RepositoryContracts;
using MediatR;

namespace Analytics.Application.Commands.Auth.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResultDto>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IApiKeyService _apiKeyService;
    private readonly IJwtProvider _jwtProvider;

    public RegisterCommandHandler(ITenantRepository tenantRepository, IUserRepository userRepository, IJwtProvider jwtProvider, IPasswordHasher passwordHasher, IApiKeyService apiKeyService)
    {
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _apiKeyService = apiKeyService;
    }

    public async Task<RegisterResultDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new BusinessRuleValidationException($"Користувач з таким {request.Email} вже існує");

        var (tenant, plainKey) = _apiKeyService.Create(request.CompanyName);
        await _tenantRepository.AddAsync(tenant);

        var passwordHasher = _passwordHasher.Hash(request.Password);
        var user = User.Create(
            tenantId: tenant.Id,
            fullName: request.FullName,
            email: request.Email,
            passwordHash: passwordHasher,
            role: UserRole.Admin
        );
        
        await _userRepository.AddAsync(user);
        
        var jwtToken = _jwtProvider.GenerateJwtToken(user);

        return new RegisterResultDto(
            TenantId: tenant.Id,
            UserId: user.Id,
            JwtToken: jwtToken,
            ApiKey: plainKey,
            Message: "Реєстрація успішна. Збережіть API ключ — він більше не буде показаний."
        );
    }
}