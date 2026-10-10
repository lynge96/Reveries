namespace Reveries.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorType { get; }
    public string Title { get; }

    protected DomainException(string title, string message)
        : base(message)
    {
        Title = title;
        ErrorType = GetType().Name;
    }
}
