using DotNetEnv;
using Npgsql;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Context;

public class PostgresDbContext : IPostgresDbContext
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public PostgresDbContext()
    {
        Env.Load();
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") 
                       ?? throw new InvalidOperationException("POSTGRES_DB environment variable is missing");
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER") 
                       ?? throw new InvalidOperationException("POSTGRES_USER environment variable is missing");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") 
                       ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is missing");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = 5432,
            Database = database,
            Username = username,
            Password = password,
            Timeout = 15,
            CommandTimeout = 20,
            Pooling = true,
            MinPoolSize = 1,
            MaxPoolSize = 20,
            ApplicationName = "Reveries PostgreSQL Database"
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