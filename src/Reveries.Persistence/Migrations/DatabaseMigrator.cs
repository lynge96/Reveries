using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;
using Reveries.Persistence.Configuration;
using Reveries.Persistence.Exceptions;

namespace Reveries.Persistence.Migrations;

public static class DatabaseMigrator
{
    public static void Run(string connectionString, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger(typeof(DatabaseMigrator));
        var pipeline = DbResiliencePipeline.BuildForStartup(logger);

        pipeline.Execute(() =>
        {
            EnsureDatabase.For.PostgresqlDatabase(connectionString, new MicrosoftUpgradeLog(logger));
            PerformUpgrade(connectionString, logger);
        });
    }

    private static void PerformUpgrade(string connectionString, ILogger logger)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .JournalToPostgresqlTable("public", "schema_versions")
            .LogTo(new MicrosoftUpgradeLog(logger))
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new DatabaseMigrationException(result.Error);
    }

    private sealed class MicrosoftUpgradeLog(ILogger logger) : IUpgradeLog
    {
        public void LogTrace(string format, params object[] args) => logger.LogTrace(format, args);

        public void LogDebug(string format, params object[] args) => logger.LogDebug(format, args);

        public void LogInformation(string format, params object[] args) => logger.LogInformation(format, args);

        public void LogWarning(string format, params object[] args) => logger.LogWarning(format, args);

        public void LogError(string format, params object[] args) => logger.LogError(format, args);

        public void LogError(Exception exception, string format, params object[] args) =>
            logger.LogError(exception, format, args);
    }
}