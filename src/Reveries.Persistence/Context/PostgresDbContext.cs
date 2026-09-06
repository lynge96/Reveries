using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Context;

public class PostgresDbContext : IDbContext
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ResiliencePipeline _resiliencePipeline;
    private readonly ILogger<PostgresDbContext> _logger;

    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;
    private bool _disposed;

    public PostgresDbContext(
        NpgsqlDataSource dataSource,
        ResiliencePipeline resiliencePipeline,
        ILogger<PostgresDbContext> logger)
    {
        _dataSource = dataSource;
        _resiliencePipeline = resiliencePipeline;
        _logger = logger;
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken ct = default) =>
        ExecuteCoreAsync((conn, cmd) => conn.QueryAsync<T>(cmd), sql, param, ct);

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CancellationToken ct = default) =>
        await ExecuteCoreAsync((conn, cmd) => conn.QueryFirstOrDefaultAsync<T>(cmd), sql, param, ct);

    public Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken ct = default) =>
        ExecuteCoreAsync((conn, cmd) => conn.QuerySingleAsync<T>(cmd), sql, param, ct);

    public Task<int> ExecuteAsync(string sql, object? param = null, CancellationToken ct = default) =>
        ExecuteCoreAsync((conn, cmd) => conn.ExecuteAsync(cmd), sql, param, ct);

    public async Task<IDbTransaction> BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));

        if (_transaction is not null)
            throw new InvalidOperationException(
                "A transaction is already active on this context; nested transactions are not supported.");

        var conn = await GetConnectionAsync(ct);
        _transaction = await conn.BeginTransactionAsync(ct);
        return _transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction == null) return;

        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) return;

        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    private async Task<TResult> ExecuteCoreAsync<TResult>(
        Func<NpgsqlConnection, CommandDefinition, Task<TResult>> operation,
        string sql,
        object? param,
        CancellationToken ct)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));

        if (_transaction is not null)
        {
            var conn = await GetConnectionAsync(ct);
            var command = new CommandDefinition(sql, param, _transaction, cancellationToken: ct);
            return await operation(conn, command);
        }

        return await _resiliencePipeline.ExecuteAsync(async token =>
        {
            var conn = await GetConnectionAsync(token);
            var command = new CommandDefinition(sql, param, cancellationToken: token);
            return await operation(conn, command);
        }, ct);
    }

    private async Task<NpgsqlConnection> GetConnectionAsync(CancellationToken ct)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(PostgresDbContext));

        if (_connection is { State: ConnectionState.Open })
            return _connection;

        if (_connection is not null && _transaction is null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }

        _connection ??= await _dataSource.OpenConnectionAsync(ct);
        return _connection;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_transaction != null)
            {
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