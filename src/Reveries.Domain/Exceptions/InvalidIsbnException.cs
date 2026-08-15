namespace Reveries.Domain;

public class InvalidIsbnException : DomainException
{
    public InvalidIsbnException(string message) : 
        base(message) 
    { }
}
