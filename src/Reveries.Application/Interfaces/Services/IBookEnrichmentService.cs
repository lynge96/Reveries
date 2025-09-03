namespace Reveries.Application.Interfaces.Services;

public interface IBookEnrichmentService
{
    Task EnrichBooksAsync(CancellationToken cancellationToken);
}