namespace Reveries.Infrastructure.Configuration;

public class LokiSettings
{
    public const string SectionName = "Loki";
    
    public string? Uri { get; init; }
    public string AppName { get; set; } = "reveries-api";
    public int BatchPostingLimit { get; set; } = 5000;
    public int QueueLimit { get; set; } = 500000;
    public int PeriodSeconds { get; set; } = 5;
}