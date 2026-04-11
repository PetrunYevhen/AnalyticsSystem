using Analytics.Application.Auth;
using Microsoft.AspNetCore.Http;

namespace Analytics.Infrastructure.Auth;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private bool _isResolved;
    private Guid _userId;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            if (!_isResolved) ResolveUserId();
            return _userId;
        }
    }

    public bool IsAuthenticated  => UserId.HasValue;


    public Guid RequiredUserId()
        => UserId ?? throw new UnauthorizedAccessException("userId claim не знайдено.");

    private void ResolveUserId()
    {
        _isResolved = true;
        var claim = _httpContextAccessor.HttpContext?.User.FindFirst("userId");
        if (claim is not null && Guid.TryParse(claim.Value, out var id))
            _userId = id;
    }
}