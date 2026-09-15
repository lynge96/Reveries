using Microsoft.AspNetCore.Diagnostics;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class ExternalDependencyExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ExternalDependencyExceptionHandler> _logger;

    public ExternalDependencyExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<ExternalDependencyExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ExternalDependencyException dependencyException)
            return false;

        _logger.LogError(dependencyException,
            "External dependency '{Dependency}' failed with upstream status {UpstreamStatus}",
            dependencyException.Dependency, dependencyException.UpstreamStatus);

        var status = (int?)dependencyException.StatusCode ?? StatusCodes.Status502BadGateway;
        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = dependencyException,
            ProblemDetails =
            {
                Title = "External Dependency Error",
                Status = status,
                Type = dependencyException.ErrorType,
                Detail = dependencyException.Message
            }
        });
    }
}