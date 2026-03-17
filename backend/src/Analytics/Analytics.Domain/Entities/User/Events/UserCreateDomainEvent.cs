using Domain.Events;

namespace Analytics.Domain.Entities.User.Events;

public class UserCreateDomainEvent : DomainEventBase
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public UserCreateDomainEvent(Guid userId, Guid tenantId)
    {}
}