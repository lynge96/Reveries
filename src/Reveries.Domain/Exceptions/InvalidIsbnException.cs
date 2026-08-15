namespace Reveries.Domain.Exceptions;

public class InvalidIsbnException : DomainException
{
    public InvalidIsbnException(string message) :
        base(message)
    { }
}
