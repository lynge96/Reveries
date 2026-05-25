using Reveries.Core.Models;

namespace Reveries.Application.Authors.Interfaces;

public interface IAuthorEnrichmentService
{
    Task EnrichAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
}