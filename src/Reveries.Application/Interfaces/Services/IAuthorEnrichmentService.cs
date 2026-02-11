using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IAuthorEnrichmentService
{
    Task EnrichAsync(IReadOnlyList<Author> authors, CancellationToken ct = default);
}