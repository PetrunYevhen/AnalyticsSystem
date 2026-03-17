using Analytics.Domain.Entities.User;

namespace Analytics.Domain.RepositoryContracts;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> FindByEmailAsync(string email);
    Task<User> GetByIdAsync(Guid userId, Guid tenantId, CancellationToken ct);
    Task UpdateAsync(User user);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
}