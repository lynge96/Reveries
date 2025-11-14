using System.Text.Json;
using Reveries.Core.Exceptions;

namespace Reveries.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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
        var path = context.Request.Path;
        var traceId = context.TraceIdentifier;

        ErrorContext errorCtx;

        switch (exception)
        {
            case ValidationException valEx:
            {
                var errors = valEx.Failures
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(f => f.ErrorMessage).ToArray());

                errorCtx = new ErrorContext(
                    Type: valEx.ErrorType,
                    StatusCode: (int)valEx.StatusCode,
                    Path: path,
                    TraceId: traceId,
                    ErrorMessage: valEx.Message,
                    ValidationErrors: errors
                );

                _logger.LogWarning(valEx, "Validation error {@Error}", errorCtx);

                context.Response.StatusCode = (int)valEx.StatusCode;
                break;
            }

            case BaseAppException appEx:
            {
                errorCtx = new ErrorContext(
                    Type: appEx.ErrorType,
                    StatusCode: (int)appEx.StatusCode,
                    Path: path,
                    TraceId: traceId,
                    ErrorMessage: appEx.Message
                );

                _logger.LogWarning(appEx, "Application error {@Error}", errorCtx);

                context.Response.StatusCode = (int)appEx.StatusCode;
                break;
            }

            default:
            {
                errorCtx = new ErrorContext(
                    Type: "Unhandled",
                    StatusCode: 500,
                    Path: path,
                    TraceId: traceId,
                    ErrorMessage: exception.Message
                );

                _logger.LogError(exception, "Unhandled exception {@Error}", errorCtx);

                context.Response.StatusCode = 500;
                break;
            }
        }

        var response = new ErrorResponse
        {
            StatusCode = context.Response.StatusCode,
            Error = errorCtx.Type,
            Message = errorCtx.ErrorMessage ?? "An error occurred while processing your request.",
            Details = errorCtx.ValidationErrors
        };

        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }   
}