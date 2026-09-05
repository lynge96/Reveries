using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace Reveries.Persistence.Migrations;

public static class DatabaseMigrator
{
    public static void Run(string connectionString, ILogger logger)
    {
        var upgrader = DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .JournalToPostgresqlTable("public", "schema_versions")
            .LogTo(new MicrosoftUpgradeLog(logger))
            .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
            throw new InvalidOperationException("Database migration failed.", result.Error);
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
