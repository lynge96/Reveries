namespace Reveries.Domain;

[Flags]
public enum DataSource
{
    Database,
    IsbndbApi,
    GoogleBooksApi,
    CombinedBookApi,
    Cache,
    Unknown
}
