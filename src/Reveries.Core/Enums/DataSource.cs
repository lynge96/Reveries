namespace Reveries.Core.Enums;

[Flags]
public enum DataSource
{
    Database,
    IsbndbApi,
    GoogleBooksApi,
    CombinedBookApi,
    Cache
}