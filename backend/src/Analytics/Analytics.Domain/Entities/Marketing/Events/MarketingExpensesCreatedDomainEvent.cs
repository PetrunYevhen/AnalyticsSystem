using Domain.Events;

namespace Analytics.Domain.Entities.Marketing.Events;

public class MarketingExpensesCreatedDomainEvent : DomainEventBase
{
    public Guid TenantId { get; }
    public Guid ExpenseId { get; }

    public MarketingExpensesCreatedDomainEvent(Guid tenantId, Guid expenseId)
    {
        TenantId = tenantId;
        ExpenseId = expenseId;
    }
}