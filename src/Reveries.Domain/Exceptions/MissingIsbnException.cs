namespace Reveries.Domain.Exceptions;

public sealed class MissingIsbnException : DomainException
{
    public MissingIsbnException()
        : base("Edition is missing an ISBN, it must have at least an ISBN-13 or an ISBN-10.") { }
}