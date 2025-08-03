using System.Data;
using Dapper;
using Reveries.Infrastructure.Persistence.ConnectionFactory;

namespace Reveries.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository
{
    protected readonly IPostgresConnectionFactory ConnectionFactory;

    protected BaseRepository(IPostgresConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    // For: Read and simple operations
    protected async Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> operation)
    {
        await using var connection = await ConnectionFactory.CreateConnectionAsync();
        return await operation(connection);
    }
    
    // For: Write, multiple and/or ACID operations
    protected async Task<T> WithTransactionAsync<T>(Func<IDbConnection, IDbTransaction, Task<T>> operation)
    {
        await using var connection = await ConnectionFactory.CreateConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var result = await operation(connection, transaction);
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    // Dapper-specific helper-functions
    protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        return await WithConnectionAsync(async connection =>
            await connection.QueryAsync<T>(sql, param));
    }

    protected async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? param = null)
    {
        return await WithConnectionAsync(async connection =>
            await connection.QuerySingleOrDefaultAsync<T>(sql, param));
    }

    protected async Task<T> QuerySingleAsync<T>(string sql, object? param = null)
    {
        return await WithConnectionAsync(async connection =>
            await connection.QuerySingleAsync<T>(sql, param));
    }

    // For complex joins with multiple types
    protected async Task<IEnumerable<TResult>> QueryMapAsync<T1, T2, TResult>(
        string sql,
        Func<T1, T2, TResult> map,
        object? param = null,
        string splitOn = "Id")
    {
        return await WithConnectionAsync(async connection =>
            await connection.QueryAsync(sql, map, param, splitOn: splitOn));
    }

    // For UPDATE, INSERT and DELETE operations
    protected async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        return await WithConnectionAsync(async connection =>
            await connection.ExecuteAsync(sql, param));
    }

    // For stored procedures
    protected async Task<T> ExecuteProcedureAsync<T>(string procName, object? param = null)
    {
        return await WithConnectionAsync(async connection =>
            await connection.QuerySingleAsync<T>(
                procName,
                param,
                commandType: CommandType.StoredProcedure));
    }

}
