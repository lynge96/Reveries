using Microsoft.AspNetCore.Mvc;
using Reveries.Application.Common.Exceptions;
using Reveries.Core.Exceptions;
using ApplicationException = Reveries.Application.Common.Exceptions.ApplicationException;

namespace Reveries.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;
    
    public ExceptionHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response has already started, cannot modify status code or headers");
            return;
        }
        
        var problemDetails = GetProblemDetails(exception, context);

        context.Response.Clear();
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["innerException"] = exception.InnerException?.Message;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private ProblemDetails GetProblemDetails(Exception exception, HttpContext context)
    {
        var path = context.Request.Path;

        ProblemDetails problemDetails;
        switch (exception)
        {
            case ExternalDependencyException depEx:
            {
                problemDetails = new ProblemDetails
                {
                    Title = "External Dependency Error",
                    Status = StatusCodes.Status502BadGateway,
                    Type = depEx.ErrorType,
                    Detail = depEx.Message,
                    Instance = path
                };

                _logger.LogError(depEx,
                    "External dependency '{Dependency}' failed with upstream status {UpstreamStatus}",
                    depEx.Dependency, depEx.UpstreamStatus);

                return problemDetails;
            }
            case ApplicationException appEx:
            {
                problemDetails = new ProblemDetails
                {
                    Title = "Application Error",
                    Status = (int)appEx.StatusCode,
                    Type = appEx.ErrorType,
                    Detail = appEx.Message,
                    Instance = path
                };

                _logger.LogWarning(appEx,
                    "Application error: {ErrorType} - {Message}",
                    appEx.ErrorType, appEx.Message);
                
                return problemDetails;
            }
            case DomainException domEx:
            {
                problemDetails = new ProblemDetails
                {
                    Title = "Domain Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Type = domEx.ErrorType,
                    Detail = domEx.Message,
                    Instance = path
                };

                _logger.LogWarning(domEx,
                    "Domain error: {ErrorType} - {Message}",
                    domEx.ErrorType, domEx.Message);
                
                return problemDetails;
            }
            default:
            {
                problemDetails = new ProblemDetails
                {
                    Title = "Unhandled Exception",
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.GetType().Name,
                    Detail = _environment.IsDevelopment() 
                        ? exception.Message 
                        : "An unexpected error occurred. Please try again later.",
                    Instance = path
                };

                _logger.LogError(exception,
                    "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
                    context.TraceIdentifier, context.Request.Path, context.Request.Method);
                
                return problemDetails;
            }
        }
    }
}