using Reveries.Application.Interfaces.Services;

namespace Reveries.BookEnrichmentWorker;

public class BookEnrichmentRunner
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BookEnrichmentRunner(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var enrichmentService = scope.ServiceProvider
            .GetRequiredService<IBookEnrichmentService>();

        await enrichmentService.EnrichBooksAsync(cancellationToken);
    }
}