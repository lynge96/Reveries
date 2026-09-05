using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Respawn;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Context;
using Reveries.Persistence.Migrations;
using Testcontainers.PostgreSql;

namespace Reveries.Persistence.Tests.Fixtures;

/// <summary>
/// Starts one throwaway PostgreSQL container for the test collection, applies the
/// schema via the DbUp migrations, and hands out <see cref="PostgresDbContext"/>
/// instances over it.
/// </summary>
public sealed class PostgresContainerFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:18")
        .Build();

    private NpgsqlDataSource? _dataSource;
    private Respawner? _respawner;

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

        DatabaseMigrator.Run(ConnectionString, NullLogger.Instance);

        await using var connection = await _dataSource.OpenConnectionAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            SchemasToInclude = ["catalog"],
            DbAdapter = DbAdapter.Postgres,
            WithReseed = true
        });
    }

    /// <summary>
    /// Wipes every row in the <c>catalog</c> schema so each test starts from an empty,
    /// known database. WithReseed resets the identity sequences so generated ids stay
    /// predictable. The DbUp journal in <c>public</c> is left untouched.
    /// </summary>
    public async Task ResetAsync()
    {
        await using var connection = await _dataSource!.OpenConnectionAsync();
        await _respawner!.ResetAsync(connection);
    }

    public async Task DisposeAsync()
    {
        try
        {
            if (_dataSource is not null)
                await _dataSource.DisposeAsync();
        }
        finally
        {
            await _container.DisposeAsync();
        }
    }
}