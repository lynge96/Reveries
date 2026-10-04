using Reveries.Application.Common.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class ExternalDependencyExceptionHandler : ProblemDetailsExceptionHandler<ExternalDependencyException>
{
    public ExternalDependencyExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ExternalDependencyExceptionHandler> logger)
        : base(problemDetailsService, logger)
    {
    }

    protected override ProblemError Map(ExternalDependencyException exception)
    {
        return new ProblemError(
            Status: (int)exception.StatusCode,
            Title: "External Dependency Error",
            ErrorCode: ProblemTypes.ToErrorCode(exception.ErrorType),
            Detail: exception.Message);
    }

    protected override void Log(HttpContext httpContext, ExternalDependencyException exception, ProblemError error)
    {
        Logger.LogError(exception,
            "External dependency '{Dependency}' failed with upstream status {UpstreamStatus}",
            exception.Dependency, exception.UpstreamStatus);
    }
}