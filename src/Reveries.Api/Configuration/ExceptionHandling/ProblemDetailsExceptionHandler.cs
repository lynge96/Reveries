using Microsoft.AspNetCore.Diagnostics;

namespace Reveries.Api.Configuration.ExceptionHandling;

public abstract class ProblemDetailsExceptionHandler<TException> : IExceptionHandler
    where TException : Exception
{
    private readonly IProblemDetailsService _problemDetailsService;

    protected ProblemDetailsExceptionHandler(IProblemDetailsService problemDetailsService, ILogger logger)
    {
        _problemDetailsService = problemDetailsService;
        Logger = logger;
    }

    protected ILogger Logger { get; }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not TException typedException)
            return false;

        var error = Map(typedException);

        Log(httpContext, typedException, error);

        httpContext.Response.StatusCode = error.Status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = typedException,
            ProblemDetails =
            {
                Title = error.Title,
                Status = error.Status,
                Type = ProblemTypes.UriFor(error.ErrorCode),
                Detail = error.Detail,
                Extensions = { ["errorCode"] = error.ErrorCode }
            }
        });
    }

    protected abstract ProblemError Map(TException exception);

    protected virtual void Log(HttpContext httpContext, TException exception, ProblemError error)
    {
        Logger.LogWarning(exception, "{ErrorCode}: {Message}", error.ErrorCode, exception.Message);
    }
}