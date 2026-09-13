using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reveries.Persistence.Exceptions;

namespace Reveries.Persistence.Migrations;

public sealed class DatabaseMigrationHostedService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;

    public DatabaseMigrationHostedService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var connectionString = _configuration.GetConnectionString(ConnectionStringKeys.ReveriesDb)
            ?? throw new MissingConnectionStringException(ConnectionStringKeys.ReveriesDb);

        DatabaseMigrator.Run(connectionString, _loggerFactory);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}