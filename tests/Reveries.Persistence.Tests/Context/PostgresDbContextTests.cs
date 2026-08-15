using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Context;

/// <summary>
/// Exercises the transaction lifecycle of PostgresDbContext against a real
/// Postgres. Each test runs on an empty database reset by the fixture.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class PostgresDbContextTests : IAsyncLifetime
{
    private readonly PostgresContainerFixture _fixture;

    public PostgresDbContextTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task BeginTransactionAsync_throws_when_a_transaction_is_already_active()
    {
        // Arrange
        await using var dbContext = _fixture.NewDbContext();
        await dbContext.BeginTransactionAsync(CancellationToken.None);

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => dbContext.BeginTransactionAsync(CancellationToken.None));
    }

    [Fact]
    public async Task BeginTransactionAsync_starts_a_new_transaction_after_the_previous_one_commits()
    {
        // Arrange
        await using var dbContext = _fixture.NewDbContext();
        await dbContext.BeginTransactionAsync(CancellationToken.None);
        await dbContext.CommitTransactionAsync(CancellationToken.None);

        // Act
        var second = await dbContext.BeginTransactionAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(second);
    }
}