using System.Net;
using Reveries.Domain.Editions;

namespace Reveries.Application.Common.Exceptions;

public class BookAlreadyExistsException : ApplicationException
{
    public BookAlreadyExistsException(Isbn isbn)
        : base($"Book with ISBN '{isbn.Value13}' already exists.", HttpStatusCode.Conflict)
    { }
}
