using System.Data;
using Npgsql;

namespace Reveries.Application.Interfaces.Persistence;

public interface IDbContext : IAsyncDisposable
{
    Task<NpgsqlConnection> GetConnectionAsync();
    Task<IDbTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    bool HasActiveTransaction { get; }
    IDbTransaction? CurrentTransaction { get; }
}
