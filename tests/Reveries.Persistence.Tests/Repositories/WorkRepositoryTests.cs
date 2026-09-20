using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Works;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class WorkRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public WorkRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetWorkById_hydrates_primary_and_secondary_genres_separately()
    {
        await using var db = _fixture.NewDbContext();
        var genres = new GenreRepository(db);
        var genreIds = await genres.AddRangeAsync(["Dystopia", "Fantasy"], CancellationToken.None);

        var works = new WorkRepository(db);
        var work = Work.Create(new WorkData(
            Title: "Nineteen Eighty-Four",
            Subtitle: null,
            AuthorIds: [],
            PrimaryGenres: ["Dystopia"],
            SecondaryGenres: ["Fantasy"],
            DeweyDecimals: [],
            Synopsis: null,
            Description: null));

        var relations = new WorkRelations(
            [genreIds["Dystopia"]],
            [genreIds["Fantasy"]],
            []);

        await works.InsertWorkAsync(work, relations, CancellationToken.None);

        var found = await works.GetWorkByIdAsync(work.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(["Dystopia"], found.Genres.Primary.Select(g => g.Name));
        Assert.Equal(["Fantasy"], found.Genres.Secondary.Select(g => g.Name));
    }

    [Fact]
    public async Task UpdateWorkSeries_persists_the_series_and_number()
    {
        await using var db = _fixture.NewDbContext();
        var works = new WorkRepository(db);
        var work = Work.Create(new WorkData(
            Title: "Dune Messiah",
            Subtitle: null,
            AuthorIds: [],
            PrimaryGenres: [],
            SecondaryGenres: [],
            DeweyDecimals: [],
            Synopsis: null,
            Description: null));
        await works.InsertWorkAsync(work, new WorkRelations([], [], []), CancellationToken.None);

        var seriesRepo = new SeriesRepository(db);
        var series = Series.Create("Dune");
        await seriesRepo.AddAsync(series, CancellationToken.None);

        work.SetSeries(series.Id, 2);
        await works.UpdateWorkSeriesAsync(work, series.Id, CancellationToken.None);

        var found = await works.GetWorkByIdAsync(work.Id, CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(series.Id, found.SeriesId);
        Assert.Equal(2, found.NumberInSeries);
    }

    [Fact]
    public async Task FindWorkIdByTitleAndAuthors_matches_on_title_and_a_shared_author()
    {
        await using var db = _fixture.NewDbContext();
        var authors = new AuthorRepository(db);
        var orwell = Author.TryCreate("George Orwell")!;
        await authors.AddRangeAsync([orwell], CancellationToken.None);

        var works = new WorkRepository(db);
        var work = NewWork("Animal Farm", [orwell.Id]);
        await works.InsertWorkAsync(work, new WorkRelations([], [], []), CancellationToken.None);

        // Case- and whitespace-insensitive title match, shared author.
        var foundId = await works.FindWorkIdByTitleAndAuthorsAsync("  animal farm ", [orwell.Id], CancellationToken.None);

        Assert.Equal(work.Id, foundId);
    }

    [Fact]
    public async Task FindWorkIdByTitleAndAuthors_does_not_match_same_title_with_a_different_author()
    {
        await using var db = _fixture.NewDbContext();
        var authors = new AuthorRepository(db);
        var homer = Author.TryCreate("Homer")!;
        var fry = Author.TryCreate("Stephen Fry")!;
        await authors.AddRangeAsync([homer, fry], CancellationToken.None);

        var works = new WorkRepository(db);
        var homersOdyssey = NewWork("The Odyssey", [homer.Id]);
        await works.InsertWorkAsync(homersOdyssey, new WorkRelations([], [], []), CancellationToken.None);

        var foundId = await works.FindWorkIdByTitleAndAuthorsAsync("The Odyssey", [fry.Id], CancellationToken.None);

        Assert.Null(foundId);
    }

    [Fact]
    public async Task FindWorkIdByTitleAndAuthors_returns_null_without_authors_to_match()
    {
        await using var db = _fixture.NewDbContext();
        var works = new WorkRepository(db);

        var foundId = await works.FindWorkIdByTitleAndAuthorsAsync("Anything", [], CancellationToken.None);

        Assert.Null(foundId);
    }

    private static Work NewWork(string title, IReadOnlyList<AuthorId> authorIds) => Work.Create(new WorkData(
        Title: title,
        Subtitle: null,
        AuthorIds: authorIds,
        PrimaryGenres: [],
        SecondaryGenres: [],
        DeweyDecimals: [],
        Synopsis: null,
        Description: null));
}