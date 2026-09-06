using Reveries.Domain.Editions;
using Reveries.Domain.Works;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class EditionRepositoryTests : IAsyncLifetime
{
    private const string Isbn13 = "9780451524935";
    private const string Isbn10 = "0451524934";

    private readonly PostgresContainerFixture _fixture;

    public EditionRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Insert_then_GetByIsbn_round_trips_and_links_the_work()
    {
        await using var db = _fixture.NewDbContext();
        var work = await InsertWorkAsync(db);
        var editions = new EditionRepository(db);
        var edition = BuildEdition(work.Id);
        await editions.InsertEditionAsync(edition, CancellationToken.None);

        var byIsbn13 = await editions.GetEditionByIsbnAsync(edition.Isbn!, CancellationToken.None);

        Assert.NotNull(byIsbn13);
        Assert.Equal(work.Id.Value, byIsbn13.WorkId.Value);
        Assert.Equal(Isbn13, byIsbn13.Isbn!.Value13);
        Assert.Equal(Isbn10, byIsbn13.Isbn!.Value10);
    }

    [Fact]
    public async Task EditionExists_reflects_whether_the_isbn_is_stored()
    {
        await using var db = _fixture.NewDbContext();
        var work = await InsertWorkAsync(db);
        var editions = new EditionRepository(db);
        var edition = BuildEdition(work.Id);

        Assert.False(await editions.EditionExistsAsync(edition.Isbn!, CancellationToken.None));

        await editions.InsertEditionAsync(edition, CancellationToken.None);

        Assert.True(await editions.EditionExistsAsync(edition.Isbn!, CancellationToken.None));
    }

    private static async Task<Work> InsertWorkAsync(Reveries.Persistence.Context.PostgresDbContext db)
    {
        var works = new WorkRepository(db);
        var work = Work.Create(new WorkData(
            Title: "Nineteen Eighty-Four",
            Subtitle: null,
            AuthorIds: [],
            PrimaryGenres: [],
            SecondaryGenres: [],
            DeweyDecimals: [],
            Synopsis: null,
            Description: null));

        await works.InsertWorkAsync(work, new WorkRelations([], [], []), CancellationToken.None);
        return work;
    }

    private static Edition BuildEdition(WorkId workId)
    {
        return Edition.Create(new EditionData(
            WorkId: workId,
            Isbn13: Isbn13,
            Isbn10: Isbn10,
            PublisherId: null,
            Pages: 328,
            PublishDate: "1949",
            LanguageIso639: "en",
            Format: null,
            EditionStatement: null,
            ImageThumbnail: null,
            ImageUrl: null,
            SaxoUrl: null,
            Dimensions: null));
    }
}