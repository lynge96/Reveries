namespace Reveries.Persistence.Exceptions;

public abstract class PersistenceException : Exception
{
    public string ErrorType { get; }

    protected PersistenceException(string message)
        : base(message)
    {
        ErrorType = GetType().Name;
    }

    protected PersistenceException(string message, Exception? innerException)
        : base(message, innerException)
    {
        ErrorType = GetType().Name;
    }
}