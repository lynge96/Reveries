using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class SeriesAlreadyExistsException : AppException
{
    public SeriesAlreadyExistsException(string? name)
        : base($"Series '{name}' already exists.", HttpStatusCode.Conflict)
    { }
}
