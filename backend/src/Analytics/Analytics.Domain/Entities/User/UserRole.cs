using Domain;

namespace Analytics.Domain.Entities.User;

public sealed class UserRole : SmartEnum<UserRole>
{
    public UserRole(string name) : base(name)
    {
    }
    
    public static readonly UserRole Admin = Add(new("Admin"));
    public static readonly UserRole Viewer = Add(new("Viewer"));
    public static readonly UserRole Analyst =  Add(new("Analyst"));
}