using Microsoft.AspNetCore.Http.Timeouts;
using Reveries.Api.Configuration.ExceptionHandling;

namespace Reveries.Api.Configuration.RequestTimeouts;

public static class RequestTimeoutExtensions
{
    public const string ExternalLookupPolicy = "ExternalLookup";

    public static IServiceCollection AddRequestTimeoutPolicies(this IServiceCollection services)
    {
        services.AddRequestTimeouts(options =>
        {
            options.AddPolicy(ExternalLookupPolicy, new RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromSeconds(15),
                TimeoutStatusCode = StatusCodes.Status504GatewayTimeout,
                WriteTimeoutResponse = WriteTimeoutProblemAsync
            });
        });

        return services;
    }

    private static async Task WriteTimeoutProblemAsync(HttpContext context)
    {
        const string errorCode = "request-timeout";
        var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();

        context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails =
            {
                Title = "Request Timeout",
                Status = StatusCodes.Status504GatewayTimeout,
                Type = ProblemTypes.UriFor(errorCode),
                Detail = "The request took too long to complete and was aborted.",
                Extensions = { ["errorCode"] = errorCode }
            }
        });
    }
}
