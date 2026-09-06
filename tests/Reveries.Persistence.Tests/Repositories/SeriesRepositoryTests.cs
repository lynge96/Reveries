using Reveries.Domain.BookSeries;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class SeriesRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public SeriesRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Add_then_GetByName_matches_case_insensitively()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new SeriesRepository(db);
        var series = Series.Create("Discworld");
        await repo.AddAsync(series, CancellationToken.None);

        var found = await repo.GetByNameAsync("discworld", CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(series.Id, found.Id);
    }

    [Fact]
    public async Task GetSeries_returns_all_rows()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new SeriesRepository(db);
        await repo.AddAsync(Series.Create("Discworld"), CancellationToken.None);
        await repo.AddAsync(Series.Create("Dune"), CancellationToken.None);

        var all = await repo.GetSeriesAsync(CancellationToken.None);

        Assert.Equal(2, all.Count);
    }
}