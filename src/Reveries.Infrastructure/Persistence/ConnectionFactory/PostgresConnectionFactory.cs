using DotNetEnv;
using Npgsql;

namespace Reveries.Infrastructure.Persistence.ConnectionFactory;

public class PostgresConnectionFactory : IPostgresConnectionFactory, IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private bool _disposed;

    public PostgresConnectionFactory()
    {
        Env.Load();
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var database = Environment.GetEnvironmentVariable("POSTGRES_DB") 
                       ?? throw new InvalidOperationException("POSTGRES_DB environment variable is missing");
        var username = Environment.GetEnvironmentVariable("POSTGRES_USER") 
                       ?? throw new InvalidOperationException("POSTGRES_USER environment variable is missing");
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") 
                       ?? throw new InvalidOperationException("POSTGRES_PASSWORD environment variable is missing");

        _connectionString = new NpgsqlConnectionStringBuilder
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
            ApplicationName = "Reveries"
        }.ToString();
    }
    
    public async Task<NpgsqlConnection> CreateConnectionAsync()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(PostgresConnectionFactory));
        }

        if (_connection == null || _connection.State == System.Data.ConnectionState.Closed)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        var retryCount = 0;
        const int maxRetries = 5;
        
        while (retryCount < maxRetries)
        {
            try
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }
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
        
        return _connection;
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        
        if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
        {
            _connection.Close();
        }
        
        _connection?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
    
}