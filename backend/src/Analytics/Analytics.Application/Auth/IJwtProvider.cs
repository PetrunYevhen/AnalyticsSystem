using Analytics.Domain.Entities.User;

namespace Analytics.Application.Auth;

public interface IJwtProvider
{
    string GenerateJwtToken(User user);
}