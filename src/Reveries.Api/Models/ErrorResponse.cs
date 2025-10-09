namespace Reveries.Api.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Error { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string? TraceId { get; set; }
}