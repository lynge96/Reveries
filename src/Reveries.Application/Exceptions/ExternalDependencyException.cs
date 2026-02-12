using System.Net;

namespace Reveries.Application.Exceptions;

public class ExternalDependencyException : ApplicationException
{
    public string Dependency { get; }
    public HttpStatusCode? UpstreamStatus { get; }

    public ExternalDependencyException(string dependency, string message, HttpStatusCode? upstreamStatus = null)
        : base(message, HttpStatusCode.ServiceUnavailable)
    {
        Dependency = dependency;
        UpstreamStatus = upstreamStatus;
    }

}