using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Infrastructure.Configuration;

namespace Reveries.Infrastructure.Persistence.Context;

public class PostgresDbContext : IDbContext
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;
    
    public PostgresDbContext(IOptions<PostgresSettings> options)
    {
        var settings = options.Value;
        _connectionString = new NpgsqlConnectionStringBuilder
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
        }.ToString();
    }
    
    public bool HasActiveTransaction => _transaction != null;
    public IDbTransaction? CurrentTransaction => _transaction;
    
    public async Task<NpgsqlConnection> GetConnectionAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));

        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State == ConnectionState.Open)
            return _connection;

        var retry = 0;
        var delay = 1000;
        while (retry < 4)
        {
            try
            {
                await _connection.OpenAsync();
                return _connection;
            }
            catch (NpgsqlException)
            {
                retry++;
                if (retry == 4) throw;
                await Task.Delay(delay);
                delay *= 2;
            }
        }
        throw new InvalidOperationException("Unable to open PostgreSQL connection.");
    }

    public async Task<IDbTransaction> BeginTransactionAsync()
    {
        if (_transaction != null)
            throw new InvalidOperationException("Transaction already in progress.");

        var conn = await GetConnectionAsync();
        _transaction = await conn.BeginTransactionAsync();
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null) return;

        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction == null) return;

        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        if (_transaction != null)
            await _transaction.DisposeAsync();

        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
                await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}