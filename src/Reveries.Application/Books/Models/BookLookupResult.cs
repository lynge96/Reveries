using Reveries.Core.Models;

namespace Reveries.Application.Books.Models;

public sealed record BookLookupResult<TKey>(
    IReadOnlyList<Book> Found,
    IReadOnlyList<TKey> NotFound)
{
    public bool HasResults => Found.Count > 0;
    public bool HasMissing => NotFound.Count > 0;
    public bool NoResults => !HasResults;
    
    public static BookLookupResult<TKey> Empty => new([], []);
}