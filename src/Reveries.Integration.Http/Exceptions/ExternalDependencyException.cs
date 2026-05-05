using System.Net;

namespace Reveries.Integration.Http.Exceptions;

public class ExternalDependencyException : Exception
{
    public string Dependency { get; }
    public int? UpstreamStatus { get; }
    public string ErrorType { get; }
    public HttpStatusCode? StatusCode { get; }

    public ExternalDependencyException(
        string dependency, 
        string message, 
        int? upstreamStatus = null,
        HttpStatusCode statusCode = HttpStatusCode.BadGateway)
        : base(message)
    {
        Dependency = dependency;
        UpstreamStatus = upstreamStatus;
        ErrorType = GetType().Name;
        StatusCode = statusCode;
    }

}