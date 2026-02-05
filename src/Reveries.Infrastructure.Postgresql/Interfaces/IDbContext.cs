using System.Data;

namespace Reveries.Infrastructure.Postgresql.Interfaces;

public interface IDbContext : IAsyncDisposable
{
    Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
