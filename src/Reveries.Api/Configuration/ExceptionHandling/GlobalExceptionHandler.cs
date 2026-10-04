namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class GlobalExceptionHandler : ProblemDetailsExceptionHandler<Exception>
{
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment,
        ILogger<GlobalExceptionHandler> logger)
        : base(problemDetailsService, logger)
    {
        _environment = environment;
    }

    protected override ProblemError Map(Exception exception)
    {
        return new ProblemError(
            Status: StatusCodes.Status500InternalServerError,
            Title: "Unhandled Exception",
            ErrorCode: "internal-server-error",
            Detail: _environment.IsDevelopment()
                ? exception.Message
                : "An unexpected error occurred. Please try again later.");
    }

    protected override void Log(HttpContext httpContext, Exception exception, ProblemError error)
    {
        Logger.LogError(exception,
            "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
            httpContext.TraceIdentifier,
            RemoveLineBreaks(httpContext.Request.Path.Value),
            RemoveLineBreaks(httpContext.Request.Method));
    }

    private static string RemoveLineBreaks(string? value) =>
        value?.Replace("\r", string.Empty).Replace("\n", string.Empty) ?? string.Empty;
}