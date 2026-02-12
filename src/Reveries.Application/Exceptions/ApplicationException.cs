using System.Net;

namespace Reveries.Application.Exceptions;

public abstract class ApplicationException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorType { get; }

    protected ApplicationException(string message, HttpStatusCode statusCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorType = GetType().Name;
    }
}