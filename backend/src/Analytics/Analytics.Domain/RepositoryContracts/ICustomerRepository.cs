using Analytics.Domain.Entities.Customer;

namespace Analytics.Domain.RepositoryContracts;

public interface ICustomerRepository
{
    Task<Customer?> GetByEmailAsync(Guid tenantId, string email, CancellationToken cancellationToken);
    Task<List<Customer>> GetByIdsAsync(Guid tenantId,List<Guid> customerIds, CancellationToken cancellationToken);
    Task AddAsync(Customer customer, CancellationToken cancellationToken);
    Task UpdateAsync(Customer customer, CancellationToken cancellationToken);
    Task UpdateRangeAsync(List<Customer> customers, CancellationToken cancellationToken);
    Task DeleteAsync(List<Customer> customer, CancellationToken cancellationToken);
}