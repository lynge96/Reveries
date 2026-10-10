using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base("Not Found", message, HttpStatusCode.NotFound)
    { }
}
