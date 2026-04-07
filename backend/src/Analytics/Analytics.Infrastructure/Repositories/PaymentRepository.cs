using Analytics.Domain.Entities.Transactions;
using Analytics.Domain.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Analytics.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AnalyticsDbContext _dbContext;

    public PaymentRepository(AnalyticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Payment payment, CancellationToken ct = default)
    {
        await _dbContext.Payments.AddAsync(payment, ct);
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid tenantId, Guid orderId, CancellationToken ct = default)
    {
        return await _dbContext.Payments
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.OrderId == orderId, ct);
    }

    public Task<Payment?> GetByOrderIdWithTransactionsAsync(Guid tenantId, Guid orderId, CancellationToken ct = default)
        => GetByOrderIdAsync(tenantId, orderId, ct);

    public async Task<Payment?> GetByIdAsync(Guid tenantId, Guid paymentId, CancellationToken ct = default)
    {
        return await _dbContext.Payments
            .Include(p => p.Transactions)
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.Id == paymentId, ct);
    }

    public Task<Payment?> GetByIdWithTransactionsAsync(Guid tenantId, Guid paymentId, CancellationToken ct)
        => GetByIdAsync(tenantId, paymentId, ct);

    public async Task AddTransactionAsync(PaymentTransaction transaction, CancellationToken ct)
    {
        await _dbContext.Set<PaymentTransaction>().AddAsync(transaction, ct);
        _dbContext.Entry(transaction).State = EntityState.Added;
    }

    public Task UpdateAsync(Payment payment, CancellationToken ct = default)
    {
        _dbContext.Payments.Update(payment);
        return Task.CompletedTask;
    }
}