using Analytics.Domain.Entities.Customer;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public CustomerRepository(AnalyticsDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Customer?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(
            c =>c.TenantId == tenantId 
                && c.Email == email, cancellationToken);
        
        return customer;
    }

    public async Task<List<Customer>> GetByIdsAsync(Guid tenantId, List<Guid> customerIds, CancellationToken cancellationToken)
    {
        return await _dbContext.Customers
                
            .Where(c => c.TenantId == tenantId && customerIds.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
    {
        await _dbContext.Customers.AddAsync(customer, cancellationToken);
    }

    public async Task UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        var entry = _dbContext.Entry(customer);
        if (entry.State == EntityState.Detached)
        {
            var tracked = await _dbContext.Customers.FindAsync(customer.Id);
            if (tracked is null) throw new InvalidOperationException($"Customer {customer.Id} not found");
            _dbContext.Entry(tracked).CurrentValues.SetValues(customer);
        }
    }

    public Task UpdateRangeAsync(List<Customer> customers, CancellationToken cancellationToken)
    {
        _dbContext.Customers.UpdateRange(customers);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(List<Customer> customer, CancellationToken cancellationToken)
    {
        _dbContext.Customers.RemoveRange(customer);
        return Task.CompletedTask;

    }
}