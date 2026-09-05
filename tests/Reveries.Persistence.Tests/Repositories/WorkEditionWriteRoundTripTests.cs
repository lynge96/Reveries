using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Services;
using Reveries.Application.Publishers.Services;
using Reveries.Domain.Authors;
using Reveries.Domain.Editions;
using Reveries.Domain.Publishers;
using Reveries.Domain.Works;
using Reveries.Persistence.Context;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Writes a work and one of its editions through a real transaction (the same
/// repository sequence the application's save flow uses — reference aggregates are
/// resolved to identities first, then the aggregates are constructed and inserted)
/// and reads them back through the views, proving the write path and its transaction
/// against real Postgres. Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class WorkEditionWriteRoundTripTests : IAsyncLifetime
{
    private const string Isbn13 = "9780451524935";
    private const string Isbn10 = "0451524934";

    private readonly PostgresContainerFixture _fixture;

    public WorkEditionWriteRoundTripTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Saving_a_work_and_edition_through_a_transaction_round_trips_through_the_views()
    {
        // Arrange & Act
        await using var writeContext = _fixture.NewDbContext();
        var (work, edition) = await PersistAsync(writeContext, CancellationToken.None);

        // Assert — read back over a fresh connection to prove it committed
        await using var readContext = _fixture.NewDbContext();

        var persistedWork = await new WorkRepository(readContext).GetWorkByIdAsync(work.Id, CancellationToken.None);
        Assert.NotNull(persistedWork);
        Assert.Equal(work.Title.Text, persistedWork.Title.Text);
        Assert.Equal(work.Subtitle, persistedWork.Subtitle);
        Assert.Equal(work.Synopsis, persistedWork.Synopsis);
        Assert.Equal(work.Description, persistedWork.Description);
        Assert.Equal(
            work.AuthorIds.OrderBy(id => id.Value),
            persistedWork.AuthorIds.OrderBy(id => id.Value));
        Assert.Equal(
            work.Genres.Primary.Select(g => g.Name).OrderBy(v => v),
            persistedWork.Genres.Primary.Select(g => g.Name).OrderBy(v => v));
        Assert.Equal(
            work.Genres.Secondary.Select(g => g.Name).OrderBy(v => v),
            persistedWork.Genres.Secondary.Select(g => g.Name).OrderBy(v => v));
        Assert.Equal(
            work.DeweyDecimals.Select(d => d.Code).OrderBy(c => c),
            persistedWork.DeweyDecimals.Select(d => d.Code).OrderBy(c => c));

        var persistedEdition = await new EditionRepository(readContext)
            .GetEditionByIsbnAsync(edition.Isbn!, CancellationToken.None);
        Assert.NotNull(persistedEdition);
        Assert.Equal(work.Id.Value, persistedEdition.WorkId.Value);
        Assert.Equal(Isbn13, persistedEdition.Isbn!.Value13);
        Assert.Equal(Isbn10, persistedEdition.Isbn!.Value10);
        Assert.Equal(edition.Pages, persistedEdition.Pages);
        Assert.Equal(edition.PublisherId, persistedEdition.PublisherId);
    }

    /// <summary>
    /// Mirrors the repository calls a save-book use case makes — reference entities are
    /// resolved to their identities via GetOrCreate first, the work (holding those ids)
    /// and its edition are constructed and inserted, then the work's genre/dewey join
    /// rows — all inside one committed transaction.
    /// </summary>
    private static async Task<(Work Work, Edition Edition)> PersistAsync(PostgresDbContext db, CancellationToken ct)
    {
        var transactionManager = new TransactionManager(db);
        var works = new WorkRepository(db);
        var editions = new EditionRepository(db);
        var authorResolver = new AuthorResolver(new AuthorRepository(db));
        var publisherResolver = new PublisherResolver(new PublisherRepository(db));
        var genreResolver = new GenreResolver(new GenreRepository(db));
        var deweyResolver = new DeweyResolver(new DeweyDecimalsRepository(db));

        await using var transaction = await transactionManager.BeginTransactionAsync(ct);

        var authorCandidates = new[] { "George Orwell", "Aldous Huxley" }
            .Select(Author.TryCreate)
            .OfType<Author>()
            .ToList();
        var authorIds = await authorResolver.ResolveIdsAsync(authorCandidates, ct);

        var publisher = await publisherResolver.ResolveAsync(Publisher.TryCreate("Signet Classics"), ct);

        var work = Work.Create(new WorkData(
            Title: "Nineteen Eighty-Four",
            Subtitle: "A Novel",
            AuthorIds: authorIds,
            PrimaryGenres: ["Dystopia"],
            SecondaryGenres: ["Fantasy"],
            DeweyDecimals: ["823"],
            Synopsis: "A dystopian novel.",
            Description: "A fuller description with more detail."));

        var edition = Edition.Create(new EditionData(
            WorkId: work.Id,
            Isbn13: Isbn13,
            Isbn10: Isbn10,
            PublisherId: publisher?.Id,
            Pages: 328,
            PublishDate: "1949",
            LanguageIso639: "en",
            Format: null,
            EditionStatement: null,
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Dimensions: null));

        var genreIds = await genreResolver.ResolveIdsAsync(work.Genres.All, ct);
        var primaryGenreIds = work.Genres.Primary.Select(g => genreIds[g.Name]).ToList();
        var secondaryGenreIds = work.Genres.Secondary.Select(g => genreIds[g.Name]).ToList();

        var deweyDecimalIds = await deweyResolver.ResolveIdsAsync(work.DeweyDecimals, ct);

        var relations = new WorkRelations(primaryGenreIds, secondaryGenreIds, deweyDecimalIds);

        await works.InsertWorkAsync(work, relations, ct);
        await editions.InsertEditionAsync(edition, ct);

        await transaction.CommitAsync(ct);

        return (work, edition);
    }
}
