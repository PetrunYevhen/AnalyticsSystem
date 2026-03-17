using Analytics.Domain.Entities.Order;

namespace Analytics.Domain.RepositoryContracts;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<bool> ExistsByExternalIdAsync(Guid tenantId, string externalId, CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(Guid tenantId, Guid orderId, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
}