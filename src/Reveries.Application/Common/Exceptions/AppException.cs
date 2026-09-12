using System.Net;

namespace Reveries.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorType { get; }

    protected AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorType = GetType().Name;
    }
}
