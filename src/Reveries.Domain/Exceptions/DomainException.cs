namespace Reveries.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorType { get; }

    protected DomainException(string message) 
        : base(message)
    {
        ErrorType = GetType().Name;
    }
}