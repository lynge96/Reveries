using System.Data;
using Npgsql;

namespace Reveries.Application.Interfaces.Persistence;

public interface IDbContext : IAsyncDisposable
{
    Task<IDbConnection> GetConnectionAsync(CancellationToken cancellationToken = default);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
