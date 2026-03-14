using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;
    private readonly IPublisher _publisher;
    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(
        DbContext context, IPublisher publisher, ILogger<UnitOfWork> logger)
    {
        _context = context;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        var domainEvents = _context.ChangeTracker
            .Entries<Entity>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        
        foreach (var domainEvent in domainEvents)
            _logger.LogInformation("[UoW] Event: {Event}", domainEvent.GetType().Name);
        
        await _context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent,  cancellationToken);
        }
        
        foreach (var entry in _context.ChangeTracker.Entries<Entity>())
        {
            entry.Entity.ClearDomainEvents();
        }
        
    }
}