using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Context;
using Reveries.Persistence.Tests.Fixtures;

namespace Reveries.Persistence.Tests.Context;

/// <summary>
/// Proves that the Polly resilience pipeline on PostgresDbContext retries transient
/// errors, ignores permanent ones, and is skipped while a transaction is active.
/// </summary>
[Collection(DatabaseCollection.Name)]
public class PostgresDbContextResilienceTests : IAsyncLifetime
{
    // SQLSTATE 40001 (serialization_failure) is one of the codes Npgsql reports as transient.
    private const string RaiseTransientSql =
        "DO $$ BEGIN RAISE EXCEPTION USING ERRCODE = '40001'; END $$;";

    // SQLSTATE 42P01 (undefined_table) is a permanent error that is never retried.
    private const string SelectFromMissingTableSql =
        "SELECT 1 FROM catalog.does_not_exist";

    private readonly PostgresContainerFixture _fixture;

    public PostgresDbContextResilienceTests(PostgresContainerFixture fixture) => _fixture = fixture;

    public Task InitializeAsync() => _fixture.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task ExecuteAsync_retries_a_transient_error_when_no_transaction_is_active()
    {
        // Arrange
        var logger = new RetryCountingLogger();
        await using var dbContext = NewDbContext(logger);

        // Act
        await Assert.ThrowsAsync<PostgresException>(
            () => dbContext.ExecuteAsync(RaiseTransientSql));

        // Assert
        Assert.Equal(3, logger.RetryCount);
    }

    [Fact]
    public async Task ExecuteAsync_does_not_retry_a_permanent_error()
    {
        // Arrange
        var logger = new RetryCountingLogger();
        await using var dbContext = NewDbContext(logger);

        // Act
        await Assert.ThrowsAsync<PostgresException>(
            () => dbContext.ExecuteAsync(SelectFromMissingTableSql));

        // Assert
        Assert.Equal(0, logger.RetryCount);
    }

    [Fact]
    public async Task ExecuteAsync_does_not_retry_a_transient_error_inside_a_transaction()
    {
        // Arrange
        var logger = new RetryCountingLogger();
        await using var dbContext = NewDbContext(logger);
        await dbContext.BeginTransactionAsync(CancellationToken.None);

        // Act
        await Assert.ThrowsAsync<PostgresException>(
            () => dbContext.ExecuteAsync(RaiseTransientSql));

        // Assert
        Assert.Equal(0, logger.RetryCount);
    }

    private PostgresDbContext NewDbContext(ILogger logger) =>
        new(_fixture.DataSource, DbResiliencePipeline.Build(logger), NullLogger<PostgresDbContext>.Instance);

    /// <summary>An ILogger that counts the warnings the retry pipeline emits on each retry.</summary>
    private sealed class RetryCountingLogger : ILogger
    {
        public int RetryCount { get; private set; }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
                RetryCount++;
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}