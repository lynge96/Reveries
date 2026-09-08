namespace Reveries.Integration.Isbndb.Configuration;

public sealed class IsbndbSettings
{
    public const string SectionName = "Isbndb";
    public const string DisplayName = "ISBNdb API";

    public required string ApiUrl { get; init; }
    public required string ApiKey { get; init; }
    public int MaxBulkIsbns { get; init; } = 100;
}