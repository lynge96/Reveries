namespace Reveries.Integration.Isbndb.Configuration;

public sealed class IsbndbSettings
{
    public string ApiUrl { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
    public int MaxBulkIsbns { get; init; } = 100;
}
