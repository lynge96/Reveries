using Dapper;
using Reveries.Domain.Authors;
using Reveries.Domain.Works;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Pins the bulk upsert behaviour of the GetOrCreate repositories: a single call
/// resolves many rows in one round-trip, tolerates duplicates within the same
/// batch (the SELECT DISTINCT guard that stops Postgres from raising
/// "ON CONFLICT DO UPDATE command cannot affect row a second time"), and stays
/// idempotent across calls. Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class GetOrCreateBatchTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public GetOrCreateBatchTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetOrCreateGenres_deduplicates_within_a_single_batch()
    {
        // Arrange — the same genre twice in one call would break a naive single-statement upsert
        var genres = new[] { Genre.TryCreate("Dystopia")!, Genre.TryCreate("Dystopia")!, Genre.TryCreate("Fantasy")! };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new GenreRepository(db).GetOrCreateGenresAsync(genres, CancellationToken.None);

        // Assert — one id per distinct name, no exception
        Assert.Equal(2, ids.Distinct().Count());
    }

    [Fact]
    public async Task GetOrCreateGenres_is_idempotent_across_calls()
    {
        // Arrange
        var genres = new[] { Genre.TryCreate("Dystopia")! };
        await using var db = _fixture.NewDbContext();
        var repository = new GenreRepository(db);

        // Act
        var first = await repository.GetOrCreateGenresAsync(genres, CancellationToken.None);
        var second = await repository.GetOrCreateGenresAsync(genres, CancellationToken.None);

        // Assert — the same row is returned, not a new one
        Assert.Equal(first, second);
    }

    [Fact]
    public async Task GetOrCreateDeweyDecimals_deduplicates_within_a_single_batch()
    {
        // Arrange
        var codes = new[] { DeweyDecimal.TryCreate("823")!, DeweyDecimal.TryCreate("823")!, DeweyDecimal.TryCreate("813")! };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new DeweyDecimalsRepository(db).GetOrCreateDeweyDecimalsAsync(codes, CancellationToken.None);

        // Assert
        Assert.Equal(2, ids.Distinct().Count());
    }

    [Fact]
    public async Task GetOrCreateAuthors_deduplicates_within_a_single_batch()
    {
        // Arrange — same author twice, resolved through the bulk upsert
        var authors = new[]
        {
            Author.TryCreate("George Orwell")!,
            Author.TryCreate("George Orwell")!,
            Author.TryCreate("Aldous Huxley")!
        };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new AuthorRepository(db).GetOrCreateAuthorsAsync(authors, CancellationToken.None);

        // Assert
        Assert.Equal(2, ids.Distinct().Count());
    }
}
