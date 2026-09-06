using Reveries.Domain.Authors;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Integration tests for the AuthorRepository primitives against real Postgres, proving the
/// hand-written SQL: citext case-insensitive lookup, conflict-safe bulk insert, and partial search.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class AuthorRepositoryTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public AuthorRepositoryTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddRange_then_GetByNames_matches_case_insensitively()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new AuthorRepository(db);
        var orwell = Author.TryCreate("George Orwell")!;
        await repo.AddRangeAsync([orwell], CancellationToken.None);

        var found = await repo.GetByNamesAsync(["george orwell"], CancellationToken.None);

        Assert.Single(found);
        Assert.Equal(orwell.Id, found[0].Id);
    }

    [Fact]
    public async Task AddRange_is_idempotent_on_the_name_conflict()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new AuthorRepository(db);
        await repo.AddRangeAsync([Author.TryCreate("George Orwell")!], CancellationToken.None);
        await repo.AddRangeAsync([Author.TryCreate("George Orwell")!], CancellationToken.None);

        var found = await repo.GetByNamesAsync(["George Orwell"], CancellationToken.None);

        Assert.Single(found);
    }

    [Fact]
    public async Task GetAuthorsByName_matches_a_partial_substring()
    {
        await using var db = _fixture.NewDbContext();
        var repo = new AuthorRepository(db);
        await repo.AddRangeAsync([Author.TryCreate("George Orwell")!], CancellationToken.None);

        var found = await repo.GetAuthorsByNameAsync(Author.TryCreate("orwell")!, CancellationToken.None);

        Assert.Single(found);
    }
}