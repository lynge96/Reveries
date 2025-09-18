namespace Reveries.Application.Common.Validation.Exceptions;

public class BookAlreadyExistsException : Exception
{
    public BookAlreadyExistsException(string message) : base(message)
    {
    }
}
