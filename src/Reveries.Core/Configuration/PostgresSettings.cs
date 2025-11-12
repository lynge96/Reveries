namespace Reveries.Core.Configuration;

public sealed class PostgresSettings
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string Database { get; init; } = null!;
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? ConnectionString { get; init; }
    public PoolSettings Pool { get; init; } = new();
    public int TimeoutSeconds { get; init; }
    public int CommandTimeoutSeconds { get; init; }
    
    public string GetConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString))
            return ConnectionString;

        return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password};" +
               $"Timeout={TimeoutSeconds};CommandTimeout={CommandTimeoutSeconds};" +
               $"Pooling=true;MinPoolSize={Pool.Min};MaxPoolSize={Pool.Max};Application Name=Reveries";
    }
    
}

public sealed class PoolSettings
{
    public int Min { get; init; }
    public int Max { get; init; }
}