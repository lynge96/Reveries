namespace Reveries.Api.Middleware;

public record ErrorContext(
    string Type,
    int StatusCode,
    string Path,
    string TraceId,
    string? ErrorMessage = null);
