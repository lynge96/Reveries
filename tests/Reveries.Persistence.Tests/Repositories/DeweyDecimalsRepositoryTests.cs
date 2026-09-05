using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

[Collection(DatabaseCollection.Name)]
public class DeweyDecimalsRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public DeweyDecimalsRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddRange_returns_generated_ids_and_GetByCodes_finds_them()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new DeweyDecimalsRepository(db);

        var created = await repo.AddRangeAsync(["823", "813"], CancellationToken.None);
        Assert.Equal(2, created.Count);

        var found = await repo.GetByCodesAsync(["823", "813"], CancellationToken.None);

        Assert.Equal(created["823"], found["823"]);
        Assert.Equal(created["813"], found["813"]);
    }
}