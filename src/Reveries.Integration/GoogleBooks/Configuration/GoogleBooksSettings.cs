namespace Reveries.Integration.GoogleBooks.Configuration;

public sealed class GoogleBooksSettings
{
    public const string SectionName = "GoogleBooks API";
    public string ApiUrl { get; init; } = null!;
    public string ApiKey { get; init; } = null!;
}