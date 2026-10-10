using System.Net;

namespace Reveries.Application.Common.Exceptions;

public sealed class ExternalDependencyException : AppException
{
    public string Dependency { get; }
    public int? UpstreamStatus { get; }

    public ExternalDependencyException(
        string dependency,
        string message,
        int? upstreamStatus = null,
        HttpStatusCode statusCode = HttpStatusCode.BadGateway,
        Exception? innerException = null)
        : base("External Dependency", message, statusCode, innerException)
    {
        Dependency = dependency;
        UpstreamStatus = upstreamStatus;
    }
}