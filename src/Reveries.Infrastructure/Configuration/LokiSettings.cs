namespace Reveries.Infrastructure.Configuration;

public class LokiSettings
{
    public const string SectionName = "Loki";

    public string? Uri { get; init; }
    public string AppName { get; init; } = "reveries-api";
    public int BatchPostingLimit { get; init; } = 5000;
    public int QueueLimit { get; init; } = 500000;
    public int PeriodSeconds { get; init; } = 5;
}
