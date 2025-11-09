using System.Net;

namespace Reveries.Blazor.BookScanner.Exceptions;

public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class ApiConnectionException : Exception
{
    public ApiConnectionException(string message, Exception? innerException = null)
        : base(message, innerException) { }
}