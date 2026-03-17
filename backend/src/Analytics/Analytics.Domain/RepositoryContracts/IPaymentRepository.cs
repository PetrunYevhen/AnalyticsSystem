using Analytics.Domain.Entities.Transactions;

namespace Analytics.Domain.RepositoryContracts;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken ct = default);
    Task<Payment?> GetByOrderIdAsync(Guid tenantId, Guid orderId, CancellationToken ct = default);
    Task<Payment?> GetByOrderIdWithTransactionsAsync(Guid tenantId, Guid orderId, CancellationToken ct = default);
    Task<Payment?> GetByIdWithTransactionsAsync(Guid tenantId, Guid paymentId, CancellationToken ct);
    Task<Payment?> GetByIdAsync(Guid tenantId, Guid paymentId, CancellationToken ct = default);
    Task AddTransactionAsync(PaymentTransaction transaction, CancellationToken ct);
    Task UpdateAsync(Payment payment, CancellationToken ct = default);
}