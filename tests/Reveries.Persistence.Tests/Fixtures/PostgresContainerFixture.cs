using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Context;
using Testcontainers.PostgreSql;

namespace Reveries.Persistence.Tests.Fixtures;

/// <summary>
/// Starts one throwaway PostgreSQL container for the test collection, applies the
/// schema, and hands out <see cref="PostgresDbContext"/> instances over it.
/// </summary>
public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18")
        .Build();

    private NpgsqlDataSource? _dataSource;

    public NpgsqlDataSource DataSource =>
        _dataSource ?? throw new InvalidOperationException("Fixture has not been initialized.");

    public string ConnectionString => _container.GetConnectionString();

    /// <summary>A fresh context over the shared container.</summary>
    public PostgresDbContext NewDbContext() =>
        new(DataSource, NullLogger<PostgresDbContext>.Instance);

    public async Task InitializeAsync()
    {
        DapperConfiguration.Configure();
        await _container.StartAsync();
        _dataSource = NpgsqlDataSource.Create(_container.GetConnectionString());
        await ApplySchemaAsync();
    }

    /// <summary>
    /// The single seam that provisions the schema. Reads the canonical
    /// <c>infra/db_schema.sql</c> dump, dropping psql meta-commands (lines starting
    /// with <c>\</c>) that the wire protocol cannot execute. Swap this one method
    /// when the schema source changes (e.g. to running DbUp migrations).
    /// </summary>
    private async Task ApplySchemaAsync()
    {
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "infra", "db_schema.sql");
        var lines = await File.ReadAllLinesAsync(schemaPath);

        var sql = string.Join(
            Environment.NewLine,
            lines.Where(line => !line.StartsWith('\\')));

        await using var command = DataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Wipes every row so each test starts from an empty, known database.
    /// RESTART IDENTITY resets the serial sequences so seeded ids stay predictable.
    /// </summary>
    public async Task ResetAsync()
    {
        const string sql = """
                           TRUNCATE TABLE
                               library.works_authors,
                               library.works_genres,
                               library.works_dewey_decimals,
                               library.editions,
                               library.works,
                               library.authors,
                               library.genres,
                               library.dewey_decimals,
                               library.publishers,
                               library.series
                           RESTART IDENTITY CASCADE
                           """;

        await using var command = DataSource.CreateCommand(sql);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DisposeAsync()
    {
        try
        {
            // Only dispose the data source if startup got far enough to create it,
            // so a failed InitializeAsync surfaces its real exception, not an NRE.
            if (_dataSource is not null)
                await _dataSource.DisposeAsync();
        }
        finally
        {
            await _container.DisposeAsync();
        }
    }
}
