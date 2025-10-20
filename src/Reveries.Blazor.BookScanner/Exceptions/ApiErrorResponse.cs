namespace Reveries.Blazor.BookScanner.Exceptions;

public class ApiErrorResponse
{
    public string? Error { get; set; }
    public string? Message { get; set; }
    public string? TraceId { get; set; }
}
