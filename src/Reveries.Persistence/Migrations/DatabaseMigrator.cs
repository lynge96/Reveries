using System.Net.Sockets;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Reveries.Persistence.Migrations;

public static class DatabaseMigrator
{
    private const int MaxAttempts = 10;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(3);

    public static void Run(string connectionString, ILogger logger)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                EnsureDatabase.For.PostgresqlDatabase(connectionString);
                PerformUpgrade(connectionString, logger);
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts && IsTransient(ex))
            {
                logger.LogWarning(
                    ex,
                    "Database not reachable yet (attempt {Attempt}/{Max}); retrying in {Delay}s.",
                    attempt, MaxAttempts, RetryDelay.TotalSeconds);
                Thread.Sleep(RetryDelay);
            }
        }
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
            throw new InvalidOperationException("Database migration failed.", result.Error);
    }

    private static bool IsTransient(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is NpgsqlException { IsTransient: true })
                return true;

            if (current is SocketException or TimeoutException)
                return true;
        }

        return false;
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