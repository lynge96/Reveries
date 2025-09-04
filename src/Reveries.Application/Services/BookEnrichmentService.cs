using Reveries.Application.Interfaces.Services;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Application.Services;

public class BookEnrichmentService : IBookEnrichmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookEnrichmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task EnrichBooksAsync(CancellationToken cancellationToken)
    {
        // TODO: Lav en Google Books client som henter data for manglende data og beskrivelse af bøgerne.
        return;
    }
}