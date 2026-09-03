using Microsoft.Extensions.Logging;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Domain.Authors;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;
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

    public async Task<EditionId> SaveBookAsync(BookCandidate candidate, CancellationToken ct)
    {
        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        await ValidateEditionNotExistsAsync(candidate.Isbn, ct);

        var (work, edition) = await BuildAggregatesAsync(candidate, ct);
        var relations = await ResolveGenreAndDeweyRelationsAsync(work, ct);

        await _works.InsertWorkAsync(work, relations, ct);
        await _editions.InsertEditionAsync(edition, ct);

        await tx.CommitAsync(ct);

        _logger.LogDebug("Saved book '{Title}' with ISBN {Isbn}.", candidate.Title, candidate.Isbn?.Value13);

        return edition.Id;
    }

    private async Task ValidateEditionNotExistsAsync(Isbn? isbn, CancellationToken ct)
    {
        if (isbn is null) return;

        if (await _editions.EditionExistsAsync(isbn, ct))
            throw new BookAlreadyExistsException(isbn);
    }

    private async Task<(Work Work, Edition Edition)> BuildAggregatesAsync(BookCandidate candidate, CancellationToken ct)
    {
        // Resolve the referenced aggregates to their identities before the aggregate is constructed
        var authors = candidate.Authors
            .Select(Author.TryCreate)
            .OfType<Author>()
            .ToList();
        var authorIds = await _authors.GetOrCreateAuthorsAsync(authors, ct);

        var publisher = await _publishers.GetOrCreateAsync(Publisher.TryCreate(candidate.Publisher), ct);

        var work = Work.Create(new WorkData(
            Title: candidate.Title,
            Subtitle: candidate.Subtitle,
            AuthorIds: authorIds,
            PrimaryGenres: candidate.PrimaryGenres,
            SecondaryGenres: candidate.SecondaryGenres,
            DeweyDecimals: candidate.DeweyDecimals,
            Synopsis: candidate.Synopsis,
            Description: candidate.Description));

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: candidate.Isbn?.Value13,
            Isbn10: candidate.Isbn?.Value10,
            PublisherId: publisher?.Id,
            Pages: candidate.Pages,
            PublishDate: candidate.PublicationDate,
            LanguageIso639: candidate.Language?.Value,
            Format: candidate.Format.ToString(),
            EditionStatement: candidate.EditionStatement,
            ImageThumbnail: candidate.Cover?.ThumbnailUrl,
            ImageUrl: candidate.Cover?.Url,
            SaxoUrl: null,
            Dimensions: candidate.Dimensions));

        return (work, edition);
    }

    private async Task<WorkRelations> ResolveGenreAndDeweyRelationsAsync(Work work, CancellationToken ct)
    {
        var genreIds = await _genres.GetOrCreateGenresAsync(work.Genres.All, ct);
        var primaryGenreIds = work.Genres.Primary.Select(g => genreIds[g.Name]).ToList();
        var secondaryGenreIds = work.Genres.Secondary.Select(g => genreIds[g.Name]).ToList();

        var deweyDecimalIds = await _deweyDecimals.GetOrCreateDeweyDecimalsAsync(work.DeweyDecimals, ct);

        return new WorkRelations(primaryGenreIds, secondaryGenreIds, deweyDecimalIds);
    }
}