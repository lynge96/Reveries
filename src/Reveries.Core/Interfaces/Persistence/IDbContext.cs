namespace Reveries.Core.Interfaces.Persistence;

public interface IDbContext : IAsyncDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
