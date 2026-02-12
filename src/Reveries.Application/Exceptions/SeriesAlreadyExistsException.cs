using System.Net;

namespace Reveries.Application.Exceptions;

public class SeriesAlreadyExistsException : ApplicationException
{
    public SeriesAlreadyExistsException(string? name) 
        : base($"Series '{name}' already exists.", HttpStatusCode.Conflict) 
    { }
}