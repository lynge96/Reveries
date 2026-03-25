using System.Net;

namespace Reveries.Application.Exceptions;

/// <summary>
/// Thrown when an entity (a book, author or publisher) cannot be found.
/// </summary>
public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound) 
    { }
}