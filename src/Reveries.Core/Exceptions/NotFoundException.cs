using System.Net;

namespace Reveries.Core.Exceptions;

/// <summary>
/// Thrown when an entity (a book, author or publisher) cannot be found.
/// </summary>
public class NotFoundException : BaseAppException
{
    public NotFoundException()
        : base("The requested resource was not found.", HttpStatusCode.NotFound) 
    { }
    
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound) 
    { }
}