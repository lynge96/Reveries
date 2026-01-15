using System.Net;

namespace Reveries.Core.Exceptions;

public class ExternalDependencyException : BaseAppException
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