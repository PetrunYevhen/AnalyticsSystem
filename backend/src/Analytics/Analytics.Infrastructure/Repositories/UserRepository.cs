using Analytics.Domain.Entities.User;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public UserRepository(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        return user;
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        var normalizedEmail = email.Trim().ToLowerInvariant();

        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);
    }

    public async Task<User> GetByIdAsync(Guid userId, Guid tenantId,  CancellationToken ct)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId, ct)
            ?? throw new Exception("Користувача не знайдено.");
        
        return user;
    }

    public Task UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email == email);
    }
}