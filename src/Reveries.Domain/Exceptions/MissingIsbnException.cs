namespace Reveries.Domain.Exceptions;

public sealed class MissingIsbnException : DomainException
{
    public MissingIsbnException()
        : base("Missing ISBN", "An edition must have at least an ISBN-13 or ISBN-10, but none was provided.") { }
}