namespace Reveries.Core.Configuration;

public class RedisSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string? Password { get; init; }
    
    public string GetConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(ConnectionString)) return ConnectionString;
        var password = string.IsNullOrWhiteSpace(Password) ? "" : $",password={Password}";
        return $"{Host}:{Port}{password}";
    }
    
}