using Analytics.Application.Auth;

namespace Analytics.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);
    }

    public bool Verify(string hash, string password)
    {
        if(string.IsNullOrWhiteSpace(hash) || string.IsNullOrWhiteSpace(password))
           return false;
        
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}