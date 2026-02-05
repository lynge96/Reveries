using Reveries.Core.Models;

namespace Reveries.Application.Interfaces.Services;

public interface IAuthorEnrichmentService
{
    Task EnrichAsync(List<Author> authors, CancellationToken ct);
}