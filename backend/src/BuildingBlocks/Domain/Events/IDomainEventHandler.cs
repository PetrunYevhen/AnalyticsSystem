using MediatR;

namespace Domain.Events;

public interface IDomainEventHandler<in T> : INotificationHandler<T>
    where T : IDomainEvent;
