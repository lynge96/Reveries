using System.Net;

namespace Reveries.Application.Exceptions;

public class BookAlreadyExistsException : ApplicationException
{
    public BookAlreadyExistsException(string isbn) 
        : base($"Book with ISBN '{isbn}' already exists.", HttpStatusCode.Conflict) 
    { }
}
