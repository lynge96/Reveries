using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class WorkPersistenceService : IWorkPersistenceService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<WorkPersistenceService> _logger;
    private readonly IAuthorEnrichmentService _authorEnrichmentService;

    private readonly IWorkRepository _works;
    private readonly IEditionRepository _editions;
    private readonly IPublisherRepository _publishers;
    private readonly IAuthorRepository _authors;
    private readonly IWorkAuthorsRepository _workAuthors;
    private readonly IGenreRepository _genres;
    private readonly IWorkGenresRepository _workGenres;
    private readonly IDeweyDecimalsRepository _deweyDecimals;
    private readonly IWorkDeweyDecimalsRepository _workDeweyDecimals;

    public WorkPersistenceService(
        ITransactionManager transactionManager,
        ILogger<WorkPersistenceService> logger,
        IAuthorEnrichmentService authorEnrichmentService,
        IWorkRepository works,
        IEditionRepository editions,
        IPublisherRepository publishers,
        IAuthorRepository authors,
        IWorkAuthorsRepository workAuthors,
        IGenreRepository genres,
        IWorkGenresRepository workGenres,
        IDeweyDecimalsRepository deweyDecimals,
        IWorkDeweyDecimalsRepository workDeweyDecimals)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _authorEnrichmentService = authorEnrichmentService;
        _works = works;
        _editions = editions;
        _publishers = publishers;
        _authors = authors;
        _workAuthors = workAuthors;
        _genres = genres;
        _workGenres = workGenres;
        _deweyDecimals = deweyDecimals;
        _workDeweyDecimals = workDeweyDecimals;
    }

    public async Task<EditionId> SaveWorkWithEditionAsync(Work work, Edition edition, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        await ValidateEditionNotExistsAsync(edition, ct);

        await _authorEnrichmentService.EnrichAsync(work.Authors, ct);

        await SaveAsync(work, edition, ct);

        await tx.CommitAsync(ct);

        return edition.Id;
    }

    private async Task ValidateEditionNotExistsAsync(Edition edition, CancellationToken ct)
    {
        var isbn = edition.Isbn13 ?? edition.Isbn10;
        if (isbn == null) return;

        if (await _editions.EditionExistsAsync(isbn, ct))
            throw new BookAlreadyExistsException(isbn);
    }

    private async Task SaveAsync(Work work, Edition edition, CancellationToken ct)
    {
        // Handle Publisher (lives on the edition)
        var publisher = await _publishers.GetOrCreateAsync(edition.Publisher, ct);
        edition.SetPublisher(publisher);

        // Insert the work and its edition
        await _works.InsertWorkAsync(work, ct);
        await _editions.InsertEditionAsync(edition, ct);

        // Handle Authors and relations (live on the work)
        var authorIds = await _authors.GetOrCreateAuthorsAsync(work.Authors, ct);
        await _workAuthors.InsertWorkAuthorsAsync(work.Id.Value, authorIds, ct);

        // Handle Genres and relations
        var genreIds = await _genres.GetOrCreateGenresAsync(work.Genres, ct);
        await _workGenres.InsertWorkGenresAsync(work.Id.Value, genreIds, ct);

        // Handle Dewey Decimals and relations
        var deweyDecimalIds = await _deweyDecimals.GetOrCreateDeweyDecimalsAsync(work.DeweyDecimals, ct);
        await _workDeweyDecimals.InsertWorkDeweyDecimalsAsync(work.Id.Value, deweyDecimalIds, ct);
    }
}