using System.Net;

namespace Reveries.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorType { get; }
    public string Title { get; }

    protected AppException(string title, string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, Exception? innerException = null)
        : base(message, innerException)
    {
        Title = title;
        StatusCode = statusCode;
        ErrorType = GetType().Name;
    }
}
