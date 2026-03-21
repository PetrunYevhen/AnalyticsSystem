namespace Analytics.Application.Auth;

public interface IUserContext
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
    Guid RequiredUserId();
}