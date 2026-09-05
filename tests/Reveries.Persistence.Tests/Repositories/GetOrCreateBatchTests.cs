using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Services;
using Reveries.Domain.Authors;
using Reveries.Domain.Works;
using Reveries.Persistence.Repositories;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Repositories;

/// <summary>
/// Pins the get-or-create behaviour of the Application resolvers over the thin repository
/// primitives: a batch is resolved by looking up the existing rows and inserting only the
/// missing ones, tolerates duplicates within the same batch (the SELECT DISTINCT guard on the
/// insert), and stays idempotent across calls. Each test runs on an empty database reset by
/// the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class GetOrCreateBatchTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public GetOrCreateBatchTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ResolveGenres_deduplicates_within_a_single_batch()
    {
        // Arrange — the same genre twice in one call
        var genres = new[] { Genre.TryCreate("Dystopia")!, Genre.TryCreate("Dystopia")!, Genre.TryCreate("Fantasy")! };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new GenreResolver(new GenreRepository(db)).ResolveIdsAsync(genres, CancellationToken.None);

        // Assert — one id per distinct name, no exception
        Assert.Equal(2, ids.Values.Distinct().Count());
    }

    [Fact]
    public async Task ResolveGenres_is_idempotent_across_calls()
    {
        // Arrange
        var genres = new[] { Genre.TryCreate("Dystopia")! };
        await using var db = _fixture.NewDbContext();
        var resolver = new GenreResolver(new GenreRepository(db));

        // Act
        var first = await resolver.ResolveIdsAsync(genres, CancellationToken.None);
        var second = await resolver.ResolveIdsAsync(genres, CancellationToken.None);

        // Assert — the same row is returned, not a new one
        Assert.Equal(first, second);
    }

    [Fact]
    public async Task ResolveDeweyDecimals_deduplicates_within_a_single_batch()
    {
        // Arrange
        var codes = new[] { DeweyDecimal.TryCreate("823")!, DeweyDecimal.TryCreate("823")!, DeweyDecimal.TryCreate("813")! };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new DeweyResolver(new DeweyDecimalsRepository(db)).ResolveIdsAsync(codes, CancellationToken.None);

        // Assert
        Assert.Equal(2, ids.Distinct().Count());
    }

    [Fact]
    public async Task ResolveAuthors_deduplicates_within_a_single_batch()
    {
        // Arrange — same author twice
        var authors = new[]
        {
            Author.TryCreate("George Orwell")!,
            Author.TryCreate("George Orwell")!,
            Author.TryCreate("Aldous Huxley")!
        };

        // Act
        await using var db = _fixture.NewDbContext();
        var ids = await new AuthorResolver(new AuthorRepository(db)).ResolveIdsAsync(authors, CancellationToken.None);

        // Assert
        Assert.Equal(2, ids.Distinct().Count());
    }
}