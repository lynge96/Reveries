namespace Reveries.Domain.Enums;

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