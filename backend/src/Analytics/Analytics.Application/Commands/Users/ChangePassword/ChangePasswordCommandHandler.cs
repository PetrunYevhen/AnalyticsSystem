using Analytics.Application.Auth;
using Analytics.Domain.RepositoryContracts;
using MediatR;

namespace Analytics.Application.Commands.Users.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly ITenantContext _tenantContext;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IPasswordHasher _passwordHasher;

    public ChangePasswordCommandHandler(ITenantContext tenantContext, IUserRepository userRepository, IUserContext userContext, IPasswordHasher passwordHasher)
    {
        _tenantContext = tenantContext;
        _userRepository = userRepository;
        _userContext = userContext;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.RequiredUserId();
        var tenantId = _tenantContext.RequiredTenantId();
        
        var user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
        
        if (!BCrypt.Net.BCrypt.Verify( request.CurrentPassword, user.PasswordHash))
            throw new Exception("Поточний пароль невірний.");

        user.ChangePassword(_passwordHasher.Hash(request.NewPassword));
        await _userRepository.UpdateAsync(user);
    }
}