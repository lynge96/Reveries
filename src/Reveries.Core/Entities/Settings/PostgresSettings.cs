namespace Reveries.Core.Entities.Settings;

public class PostgresSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 5432;
    public string Database { get; set; } = "";
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public int Timeout { get; set; } = 15;
    public int CommandTimeout { get; set; } = 30;
    public bool Pooling { get; set; } = true;
    public int MinPoolSize { get; set; } = 1;
    public int MaxPoolSize { get; set; } = 100;
}