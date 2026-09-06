using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class GenreRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public GenreRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddRange_returns_generated_ids_and_GetByNames_finds_them()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new GenreRepository(db);

        var created = await repo.AddRangeAsync(["Dystopia", "Fantasy"], CancellationToken.None);
        Assert.Equal(2, created.Count);

        var found = await repo.GetByNamesAsync(["Dystopia", "Fantasy"], CancellationToken.None);

        Assert.Equal(created["Dystopia"], found["Dystopia"]);
        Assert.Equal(created["Fantasy"], found["Fantasy"]);
    }

    [Fact]
    public async Task AddRange_deduplicates_within_a_single_batch()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new GenreRepository(db);

        var created = await repo.AddRangeAsync(["Fiction", "Fiction"], CancellationToken.None);

        Assert.Single(created);
    }
}