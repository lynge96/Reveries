using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;

namespace Reveries.Application.Books.Interfaces;

public interface ICoverImageStore
{
    Task<StoredCover?> IngestAsync(Cover? source, Isbn? isbn, CancellationToken ct);
}
