using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Reveries.Persistence.Configuration;

public static class DbResiliencePipeline
{
    private const int QueryRetryAttempts = 3;
    private static readonly TimeSpan QueryBaseDelay = TimeSpan.FromMilliseconds(200);

    private const int StartupRetryAttempts = 10;
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(3);

    public static ResiliencePipeline Build(ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = TransientErrors(),
                MaxRetryAttempts = QueryRetryAttempts,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = QueryBaseDelay,
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Transient database error; retry {Attempt}/{Max} in {Delay}.",
                        args.AttemptNumber + 1, QueryRetryAttempts, args.RetryDelay);
                    return default;
                }
            })
            .Build();
    }

    public static ResiliencePipeline BuildForStartup(ILogger logger)
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = TransientErrors(),
                MaxRetryAttempts = StartupRetryAttempts,
                BackoffType = DelayBackoffType.Constant,
                Delay = StartupDelay,
                OnRetry = args =>
                {
                    logger.LogWarning(
                        args.Outcome.Exception,
                        "Database not reachable yet (attempt {Attempt}/{Max}); retrying in {Delay}.",
                        args.AttemptNumber + 1, StartupRetryAttempts, args.RetryDelay);
                    return default;
                }
            })
            .Build();
    }

    private static PredicateBuilder<object> TransientErrors()
    {
        return new PredicateBuilder().Handle<Exception>(IsTransient);
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
}