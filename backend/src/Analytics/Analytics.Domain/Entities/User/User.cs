using Analytics.Domain.Entities.User.Events;
using Domain;

namespace Analytics.Domain.Entities.User;

public class User : Entity
{
    public string FullName  { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; } = null!;

    private User(){}
    
    public static User Create(
        Guid tenantId, 
        string fullName, 
        string email, 
        string passwordHash, 
        UserRole role)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FullName = fullName,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
        
        user.AddDomainEvent(new UserCreateDomainEvent(user.Id, user.TenantId));
        return user;
    }
    
    public void ChangePassword(string newHash)
    {
        PasswordHash = newHash;
    }
}