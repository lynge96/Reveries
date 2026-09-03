using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;

namespace Reveries.Application.Books.Services;

public class WorkPersistenceService : IWorkPersistenceService
{
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger<WorkPersistenceService> _logger;

    private readonly IWorkRepository _works;
    private readonly IEditionRepository _editions;
    private readonly IPublisherRepository _publishers;
    private readonly IAuthorRepository _authors;
    private readonly IGenreRepository _genres;
    private readonly IDeweyDecimalsRepository _deweyDecimals;

    public WorkPersistenceService(
        ITransactionManager transactionManager,
        ILogger<WorkPersistenceService> logger,
        IWorkRepository works,
        IEditionRepository editions,
        IPublisherRepository publishers,
        IAuthorRepository authors,
        IGenreRepository genres,
        IDeweyDecimalsRepository deweyDecimals)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _works = works;
        _editions = editions;
        _publishers = publishers;
        _authors = authors;
        _genres = genres;
        _deweyDecimals = deweyDecimals;
    }

    public async Task<EditionId> SaveWorkWithEditionAsync(Work work, Edition edition, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        await ValidateEditionNotExistsAsync(edition, ct);

        await SaveAsync(work, edition, ct);

        await tx.CommitAsync(ct);

        return edition.Id;
    }

    private async Task ValidateEditionNotExistsAsync(Edition edition, CancellationToken ct)
    {
        var isbn = edition.Isbn;
        if (isbn == null) return;

        if (await _editions.EditionExistsAsync(isbn, ct))
            throw new BookAlreadyExistsException(isbn);
    }

    private async Task SaveAsync(Work work, Edition edition, CancellationToken ct)
    {
        // Publisher lives on the edition
        var publisher = await _publishers.GetOrCreateAsync(edition.Publisher, ct);
        edition.SetPublisher(publisher);

        // Resolve the referenced aggregates the work links to
        var relations = await ResolveRelationsAsync(work, ct);

        // The work owns its link rows; the repository persists them together
        await _works.InsertWorkAsync(work, relations, ct);
        await _editions.InsertEditionAsync(edition, ct);
    }

    private async Task<WorkRelations> ResolveRelationsAsync(Work work, CancellationToken ct)
    {
        var authorIds = await _authors.GetOrCreateAuthorsAsync(work.Authors, ct);

        var genreIds = await _genres.GetOrCreateGenresAsync(work.Genres.All, ct);
        var primaryGenreIds = work.Genres.Primary.Select(g => genreIds[g.Name]).ToList();
        var secondaryGenreIds = work.Genres.Secondary.Select(g => genreIds[g.Name]).ToList();

        var deweyDecimalIds = await _deweyDecimals.GetOrCreateDeweyDecimalsAsync(work.DeweyDecimals, ct);

        return new WorkRelations(authorIds, primaryGenreIds, secondaryGenreIds, deweyDecimalIds);
    }
}