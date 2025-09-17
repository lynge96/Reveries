namespace Reveries.Application.Common.Exceptions;

public class BookAlreadyExistsException : Exception
{
    public BookAlreadyExistsException(string message) : base(message)
    {
    }
}
