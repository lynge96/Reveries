using System.Net;

namespace Reveries.Core.Exceptions;

public class SeriesAlreadyExistsException : BaseAppException
{
    public SeriesAlreadyExistsException(string? name) 
        : base($"Series '{name}' already exists.", HttpStatusCode.Conflict) 
    { }
}