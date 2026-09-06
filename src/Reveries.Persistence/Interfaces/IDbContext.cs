using System.Data;

namespace Reveries.Persistence.Interfaces;

public interface IDbContext : IAsyncDisposable
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default);
    Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default);
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}