using Reveries.Application.Books.Models;

namespace Reveries.Application.Books.Interfaces;

public interface ICoverImageStore
{
    Task<StoredCover?> IngestAsync(string? sourceUrl, CancellationToken ct);
}