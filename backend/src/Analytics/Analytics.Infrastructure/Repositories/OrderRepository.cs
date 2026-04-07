using Analytics.Domain.Entities.Order;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AnalyticsDbContext _context;

    public OrderRepository(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<bool> ExistsByExternalIdAsync(Guid tenantId, string externalId, CancellationToken cancellationToken)
    {
        return await _context.Orders.AnyAsync(
            o => o.TenantId == tenantId 
                 && o.ExternalOrderId == externalId, 
            cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(Guid tenantId, Guid orderId, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.TenantId == tenantId
        && o.Id == orderId, cancellationToken);

        return order;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken)
    {
        _context.Orders.Update(order);
        return Task.CompletedTask;
    }
}