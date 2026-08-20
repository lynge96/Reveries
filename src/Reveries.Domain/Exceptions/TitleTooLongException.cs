namespace Reveries.Domain.Exceptions;

public sealed class TitleTooLongException : DomainException
{
    public TitleTooLongException(int length, int maxLength)
        : base($"Title cannot exceed {maxLength} characters, but was {length}.") { }
}