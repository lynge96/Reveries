using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

/// <summary>
/// Resolves the Saxo URL for a book by its ISBN, or null when no URL can be produced.
/// The current implementation builds an ISBN search deep link and does not verify that the book exists on Saxo.
/// </summary>
public interface ISaxoBookSearch
{
    Task<SaxoUrl?> FindBookUrlAsync(Isbn isbn, CancellationToken ct = default);
}