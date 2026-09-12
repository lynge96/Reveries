using Microsoft.Extensions.Http.Resilience;

namespace Reveries.Integration.Http;

public static class ResilienceConfiguration
{
    public static void Configure(HttpStandardResilienceOptions options)
    {
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(20);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(60);
    }
}