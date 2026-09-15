using Microsoft.AspNetCore.Diagnostics;
using Reveries.Domain.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class DomainExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<DomainExceptionHandler> _logger;

    public DomainExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<DomainExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
            return false;

        _logger.LogWarning(domainException,
            "Domain error: {ErrorType} - {Message}",
            domainException.ErrorType, domainException.Message);

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = domainException,
            ProblemDetails =
            {
                Title = "Domain Validation Error",
                Status = StatusCodes.Status400BadRequest,
                Type = domainException.ErrorType,
                Detail = domainException.Message
            }
        });
    }
}