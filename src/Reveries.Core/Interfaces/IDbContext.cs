namespace Reveries.Core.Interfaces;

public interface IDbContext : IAsyncDisposable
{
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
