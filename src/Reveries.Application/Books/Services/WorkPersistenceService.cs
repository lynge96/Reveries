using Microsoft.Extensions.Logging;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.Common.Abstractions;
using Reveries.Application.Common.Exceptions;
using Reveries.Application.Publishers.Interfaces;
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
    private readonly IAuthorResolver _authorResolver;
    private readonly IPublisherResolver _publisherResolver;
    private readonly IGenreResolver _genreResolver;
    private readonly IDeweyResolver _deweyResolver;
    private readonly ISaxoBookSearch _saxoBookSearch;

    public WorkPersistenceService(
        ITransactionManager transactionManager,
        ILogger<WorkPersistenceService> logger,
        IWorkRepository works,
        IEditionRepository editions,
        IAuthorResolver authorResolver,
        IPublisherResolver publisherResolver,
        IGenreResolver genreResolver,
        IDeweyResolver deweyResolver,
        ISaxoBookSearch saxoBookSearch)
    {
        _transactionManager = transactionManager;
        _logger = logger;
        _works = works;
        _editions = editions;
        _authorResolver = authorResolver;
        _publisherResolver = publisherResolver;
        _genreResolver = genreResolver;
        _deweyResolver = deweyResolver;
        _saxoBookSearch = saxoBookSearch;
    }

    public async Task<EditionId> SaveBookAsync(BookCandidate candidate, CancellationToken ct)
    {
        // Resolve the Saxo link before opening the transaction: it is an external-facing lookup and
        // must not run inside the DB transaction (a resolved candidate link is reused when present).
        var saxoUrl = candidate.SaxoUrl ?? await ResolveSaxoUrlAsync(candidate.Isbn, ct);

        await using var tx = await _transactionManager.BeginTransactionAsync(ct);

        await ValidateEditionNotExistsAsync(candidate.Isbn, ct);

        // Resolve authors first: their identities are both the new work's authors and the signal
        // the de-duplication match keys on (same normalized title + a shared author).
        var authorIds = await ResolveAuthorIdsAsync(candidate, ct);
        var workId = await ResolveWorkAsync(candidate, authorIds, ct);

        var publisher = await _publisherResolver.ResolveAsync(Publisher.TryCreate(candidate.Publisher), ct);
        var edition = BuildEdition(candidate, workId, publisher?.Id, saxoUrl);
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

    private async Task<List<AuthorId>> ResolveAuthorIdsAsync(BookCandidate candidate, CancellationToken ct)
    {
        var authors = candidate.Authors
            .Select(Author.TryCreate)
            .OfType<Author>()
            .ToList();

        return await _authorResolver.ResolveIdsAsync(authors, ct);
    }

    private async Task<WorkId> ResolveWorkAsync(BookCandidate candidate, IReadOnlyList<AuthorId> authorIds, CancellationToken ct)
    {
        // De-duplicate only when we have an author to match on: a same-title work is treated as the
        // same work only if it also shares an author, so same-title/different-author works stay
        // distinct. A book without authors is never de-duplicated (it is inserted as a new work).
        if (authorIds.Count > 0)
        {
            var existingId = await _works.FindWorkIdByTitleAndAuthorsAsync(candidate.Title, authorIds, ct);
            if (existingId is { } id)
            {
                _logger.LogDebug("Reusing existing work {WorkId} matching title '{Title}' and a shared author.", id.Value, candidate.Title);
                return id;
            }
        }

        return await CreateWorkAsync(candidate, authorIds, ct);
    }

    private async Task<WorkId> CreateWorkAsync(BookCandidate candidate, IReadOnlyList<AuthorId> authorIds, CancellationToken ct)
    {
        var work = Work.Create(new WorkData(
            Title: candidate.Title,
            Subtitle: candidate.Subtitle,
            AuthorIds: authorIds,
            PrimaryGenres: candidate.PrimaryGenres,
            SecondaryGenres: candidate.SecondaryGenres,
            DeweyDecimals: candidate.DeweyDecimals,
            Synopsis: candidate.Synopsis,
            Description: candidate.Description));

        var relations = await ResolveGenreAndDeweyRelationsAsync(work, ct);

        await _works.InsertWorkAsync(work, relations, ct);

        return work.Id;
    }

    private async Task<SaxoUrl?> ResolveSaxoUrlAsync(Isbn? isbn, CancellationToken ct)
    {
        if (isbn is null)
            return null;

        try
        {
            return await _saxoBookSearch.FindBookUrlAsync(isbn, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Saxo link resolution failed for ISBN {Isbn}; saving the book without one.", isbn.Value13);
            return null;
        }
    }

    private static Edition BuildEdition(BookCandidate candidate, WorkId workId, PublisherId? publisherId, SaxoUrl? saxoUrl)
    {
        return Edition.Create(new EditionData(
            WorkId: workId,
            Isbn13: candidate.Isbn?.Value13,
            Isbn10: candidate.Isbn?.Value10,
            PublisherId: publisherId,
            Pages: candidate.Pages,
            PublishDate: candidate.PublicationDate,
            LanguageIso639: candidate.Language?.Value,
            Format: candidate.Format.ToString(),
            EditionStatement: candidate.EditionStatement,
            ImageThumbnail: candidate.Cover?.ThumbnailUrl,
            ImageUrl: candidate.Cover?.Url,
            SaxoUrl: saxoUrl?.Value,
            Dimensions: candidate.Dimensions));
    }

    private async Task<WorkRelations> ResolveGenreAndDeweyRelationsAsync(Work work, CancellationToken ct)
    {
        var genreIds = await _genreResolver.ResolveIdsAsync(work.Genres.All, ct);
        var primaryGenreIds = work.Genres.Primary.Select(g => genreIds[g.Name]).ToList();
        var secondaryGenreIds = work.Genres.Secondary.Select(g => genreIds[g.Name]).ToList();

        var deweyDecimalIds = await _deweyResolver.ResolveIdsAsync(work.DeweyDecimals, ct);

        return new WorkRelations(primaryGenreIds, secondaryGenreIds, deweyDecimalIds);
    }
}