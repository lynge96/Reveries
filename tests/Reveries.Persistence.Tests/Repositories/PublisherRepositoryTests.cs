using Reveries.Domain.Publishers;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class PublisherRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public PublisherRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Add_then_GetByName_matches_case_insensitively()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new PublisherRepository(db);
        var penguin = Publisher.TryCreate("Penguin")!;
        await repo.AddAsync(penguin, CancellationToken.None);

        var found = await repo.GetByNameAsync("penguin", CancellationToken.None);

        Assert.NotNull(found);
        Assert.Equal(penguin.Id, found.Id);
    }

    [Fact]
    public async Task GetByName_returns_null_when_absent()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new PublisherRepository(db);

        var found = await repo.GetByNameAsync("Nonexistent", CancellationToken.None);

        Assert.Null(found);
    }

    [Fact]
    public async Task SearchByName_matches_a_partial_substring()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new PublisherRepository(db);
        await repo.AddAsync(Publisher.TryCreate("Signet Classics")!, CancellationToken.None);

        var found = await repo.SearchByNameAsync(Publisher.TryCreate("signet")!, CancellationToken.None);

        Assert.Single(found);
    }
}