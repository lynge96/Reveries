using Microsoft.AspNetCore.Diagnostics;
using Reveries.Application.Common.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class ApplicationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<ApplicationExceptionHandler> _logger;

    public ApplicationExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<ApplicationExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not AppException applicationException)
            return false;

        _logger.LogWarning(applicationException,
            "Application error: {ErrorType} - {Message}",
            applicationException.ErrorType, applicationException.Message);

        var status = (int)applicationException.StatusCode;
        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = applicationException,
            ProblemDetails =
            {
                Title = "Application Error",
                Status = status,
                Type = applicationException.ErrorType,
                Detail = applicationException.Message
            }
        });
    }
}