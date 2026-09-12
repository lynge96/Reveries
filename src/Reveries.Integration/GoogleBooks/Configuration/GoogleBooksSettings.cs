namespace Reveries.Integration.GoogleBooks.Configuration;

public sealed class GoogleBooksSettings
{
    public const string SectionName = "GoogleBooks";
    public const string DisplayName = "GoogleBooks API";

    public required string ApiUrl { get; init; }
    public string? ApiKey { get; init; }
}