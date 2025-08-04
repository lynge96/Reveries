using Npgsql;

namespace Reveries.Infrastructure.Persistence.Context;

public interface IPostgresDbContext : IAsyncDisposable
{
    Task<NpgsqlConnection> GetConnectionAsync();
    Task<NpgsqlTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}