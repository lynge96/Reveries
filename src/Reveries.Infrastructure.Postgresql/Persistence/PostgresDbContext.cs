using System.Data;
using Microsoft.Extensions.Logging;
using Npgsql;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence;

public class PostgresDbContext : IDbContext
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<PostgresDbContext> _logger;
    
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public PostgresDbContext(NpgsqlDataSource dataSource, ILogger<PostgresDbContext> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }
    
    public bool HasActiveTransaction => _transaction != null;
    public IDbTransaction? CurrentTransaction => _transaction;
    
    public async Task<IDbConnection> GetConnectionAsync(CancellationToken ct = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));

        if (_connection is { State: ConnectionState.Open }) return _connection;

        if (_connection is null)
        {
            _connection = await _dataSource.OpenConnectionAsync(ct);
        }
        else if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(ct);
        }

        return _connection!;
    }

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Beginning transaction.");
        
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));
        if (_transaction != null) 
            return _transaction;

        var conn = (NpgsqlConnection)await GetConnectionAsync(ct);
        _transaction = await conn.BeginTransactionAsync(ct);
        return _transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Committing transaction.");
        
        if (_transaction == null) return;
        
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        _logger.LogDebug("Rolling back transaction.");
        
        if (_transaction is null) return;
        
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_transaction != null)
            {
                _logger.LogWarning("Transaction disposed without commit. Rolling back.");

                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
            }

            if (_connection != null)
            {
                await _connection.DisposeAsync();
            }
        }
        finally
        {
            _transaction = null;
            _connection = null;
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}