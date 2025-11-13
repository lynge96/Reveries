using System.Net;

namespace Reveries.Core.Exceptions;

public class ExternalDependencyException : BaseAppException
{
    public string Dependency { get; }
    public HttpStatusCode? UpstreamStatus { get; }

    public ExternalDependencyException(string dependency, string message, HttpStatusCode? upstreamStatus = null, Exception? innerException = null)
        : base(message, HttpStatusCode.BadGateway, "ExternalDependencyError")
    {
        Dependency = dependency;
        UpstreamStatus = upstreamStatus;
    }

}