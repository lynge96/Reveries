using System.Net;
using Reveries.Domain.Editions;

namespace Reveries.Application.Common.Exceptions;

public class BookAlreadyExistsException : AppException
{
    public BookAlreadyExistsException(Isbn isbn)
        : base("Book Already Exists", $"A book with ISBN '{isbn.Value13}' already exists in the catalog.", HttpStatusCode.Conflict)
    { }
}
