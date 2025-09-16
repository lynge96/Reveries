using DotNetEnv;
using Microsoft.Extensions.Options;
using Npgsql;
using Reveries.Infrastructure.Configuration;
using Reveries.Infrastructure.Persistence.Interfaces;

namespace Reveries.Infrastructure.Persistence.Context;

public class PostgresDbContext : IPostgresDbContext
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public PostgresDbContext(IOptions<PostgresSettings> options)
    {
        var settings = options.Value;
        
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = settings.Host,
            Port = settings.Port,
            Database = settings.Database,
            Username = settings.Username,
            Password = settings.Password,
            Timeout = settings.Timeout,
            CommandTimeout = settings.CommandTimeout,
            Pooling = settings.Pooling,
            MinPoolSize = settings.MinPoolSize,
            MaxPoolSize = settings.MaxPoolSize,
            ApplicationName = "Reveries PostgreSQL Database",
            IncludeErrorDetail = true // For debugging purposes only
        };

        _connectionString = builder.ToString();
    }
    
    public bool HasActiveTransaction => _transaction != null;

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PostgresDbContext));
        }
    }

    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PostgresDbContext));
        }

        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State != System.Data.ConnectionState.Open)
        {
            await RetryConnectionAsync();
        }

        return _connection;
    }
    
    private async Task RetryConnectionAsync()
    {
        var retryCount = 0;
        const int maxRetries = 5;

        while (retryCount < maxRetries)
        {
            try
            {
                await _connection!.OpenAsync();
                break;
            }
            catch (NpgsqlException)
            {
                retryCount++;
                if (retryCount == maxRetries)
                    throw;

                await Task.Delay(TimeSpan.FromSeconds(2 * retryCount));
            }
        }
    }
    
    public async Task BeginTransactionAsync()
    {
        ThrowIfDisposed();
        
        if (HasActiveTransaction)
        {
            throw new InvalidOperationException("Transaction already in progress");
        }

        var connection = await GetConnectionAsync();
        _transaction = await connection.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        ThrowIfDisposed();

        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    public async Task RollbackTransactionAsync()
    {
        ThrowIfDisposed();

        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }

        if (_connection != null)
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }
            await _connection.DisposeAsync();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
    
    
}