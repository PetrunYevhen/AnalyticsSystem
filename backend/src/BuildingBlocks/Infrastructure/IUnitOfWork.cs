namespace Infrastructure;

public interface IUnitOfWork
{
    Task CommitAsync(
        CancellationToken cancellationToken);
}