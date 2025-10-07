using System.Net;

namespace Reveries.Core.Exceptions;

public class BookAlreadyExistsException : BaseAppException
{
    public BookAlreadyExistsException(string isbn) 
        : base($"Book with ISBN '{isbn}' already exists.", HttpStatusCode.Conflict, "BookAlreadyExists") { }
}
