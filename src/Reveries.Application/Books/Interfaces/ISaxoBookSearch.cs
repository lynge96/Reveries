using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// Resolves the canonical Saxo product URL for a book by its ISBN, or null when the book is not found on Saxo.
/// </summary>
public interface ISaxoBookSearch
{
    Task<SaxoUrl?> FindBookUrlAsync(Isbn isbn, CancellationToken ct = default);
}