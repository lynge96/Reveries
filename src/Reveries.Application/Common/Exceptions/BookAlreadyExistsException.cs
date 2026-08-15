using System.Net;
using Reveries.Domain;

namespace Reveries.Application.Common.Exceptions;

public class BookAlreadyExistsException : ApplicationException
{
    public BookAlreadyExistsException(Isbn isbn) 
        : base($"Book with ISBN '{isbn.Value}' already exists.", HttpStatusCode.Conflict) 
    { }
}
