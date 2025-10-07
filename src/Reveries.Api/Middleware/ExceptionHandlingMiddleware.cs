using System.Net;
using System.Text.Json;
using Reveries.Api.Models;
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
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            TraceId = context.TraceIdentifier
        };

        if (exception is BaseAppException appEx)
        {
            context.Response.StatusCode = (int)appEx.StatusCode;
            errorResponse.StatusCode = (int)appEx.StatusCode;
            errorResponse.Error = appEx.ErrorType;
            errorResponse.Message = appEx.Message;
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
            errorResponse.Error = "ServerError";
            errorResponse.Message = "An unexpected error occurred.";

            _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}