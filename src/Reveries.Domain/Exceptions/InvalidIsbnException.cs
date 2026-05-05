namespace Reveries.Core.Exceptions;

public class InvalidIsbnException : DomainException
{
    public InvalidIsbnException(string message) : 
        base(message) 
    { }
}
