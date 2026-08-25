using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Domain.Works;
using Reveries.Persistence.Context;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Writes a work and one of its editions through a real transaction (the same
/// repository sequence the application's save flow uses) and reads them back
/// through the views, proving the write path and its transaction against real
/// Postgres. Each test runs on an empty database reset by the fixture.
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
        // Arrange
        var work = NewWork();
        var edition = NewEdition(work.Id);
        await using var writeContext = _fixture.NewDbContext();

        // Act
        await PersistAsync(writeContext, work, edition, CancellationToken.None);

        // Assert — read back over a fresh connection to prove it committed
        await using var readContext = _fixture.NewDbContext();

        var persistedWork = await new WorkRepository(readContext).GetWorkByIdAsync(work.Id, CancellationToken.None);
        Assert.NotNull(persistedWork);
        Assert.Equal(work.Title.Text, persistedWork.Title.Text);
        Assert.Equal(work.Synopsis, persistedWork.Synopsis);
        Assert.Equal(
            work.Authors.Select(a => a.NormalizedName).OrderBy(n => n),
            persistedWork.Authors.Select(a => a.NormalizedName).OrderBy(n => n));
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
        Assert.Equal(edition.Publisher!.Name, persistedEdition.Publisher!.Name);
        Assert.Equal(DataSource.Database, persistedEdition.DataSource);
    }

    /// <summary>
    /// Mirrors the repository calls a save-work use case makes — reference entities
    /// are resolved via GetOrCreate, the work and edition rows are inserted, then the
    /// work's join rows — all inside one committed transaction.
    /// </summary>
    private static async Task PersistAsync(PostgresDbContext db, Work work, Edition edition, CancellationToken ct)
    {
        var transactionManager = new TransactionManager(db);
        var works = new WorkRepository(db);
        var editions = new EditionRepository(db);
        var publishers = new PublisherRepository(db);
        var authors = new AuthorRepository(db);
        var genres = new GenreRepository(db);
        var deweyDecimals = new DeweyDecimalsRepository(db);

        await using var transaction = await transactionManager.BeginTransactionAsync(ct);

        var publisher = await publishers.GetOrCreateAsync(edition.Publisher, ct);
        edition.SetPublisher(publisher);

        var authorIds = await authors.GetOrCreateAuthorsAsync(work.Authors, ct);

        var genreIds = await genres.GetOrCreateGenresAsync(work.Genres.All, ct);
        var primaryGenreIds = work.Genres.Primary.Select(g => genreIds[g.Name]).ToList();
        var secondaryGenreIds = work.Genres.Secondary.Select(g => genreIds[g.Name]).ToList();

        var deweyDecimalIds = await deweyDecimals.GetOrCreateDeweyDecimalsAsync(work.DeweyDecimals, ct);

        var relations = new WorkRelations(authorIds, primaryGenreIds, secondaryGenreIds, deweyDecimalIds);

        await works.InsertWorkAsync(work, relations, ct);
        await editions.InsertEditionAsync(edition, ct);

        await transaction.CommitAsync(ct);
    }

    private static Work NewWork() => Work.Create(
        title: "Nineteen Eighty-Four",
        authors: ["George Orwell", "Aldous Huxley"],
        primaryGenres: ["Dystopia"],
        secondaryGenres: ["Fantasy"],
        deweyDecimals: ["823"],
        synopsis: "A dystopian novel.");

    private static Edition NewEdition(WorkId workId) => Edition.Create(new EditionData(
        WorkId: workId,
        Isbn13: Isbn13,
        Isbn10: Isbn10,
        Publisher: "Signet Classics",
        Pages: 328,
        PublishDate: "1949",
        LanguageIso639: "en",
        Binding: null,
        EditionStatement: null,
        ImageThumbnail: null,
        ImageUrl: null,
        SaxoUrl: null,
        Msrp: null,
        Dimensions: null,
        DataSource: DataSource.Database));
}