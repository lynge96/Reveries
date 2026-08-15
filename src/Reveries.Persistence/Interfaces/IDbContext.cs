using System.Data;

namespace Reveries.Persistence.Interfaces;

public interface IDbContext : IAsyncDisposable
{
    IDbTransaction? CurrentTransaction { get; }
    Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
