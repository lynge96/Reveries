using System.Net;

namespace Reveries.Core.Exceptions;

public abstract class BaseAppException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ErrorType { get; }

    protected BaseAppException(string message, HttpStatusCode statusCode) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorType = GetType().Name;
    }
}