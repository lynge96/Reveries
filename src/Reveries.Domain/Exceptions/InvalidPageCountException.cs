namespace Reveries.Core.Exceptions;

public sealed class InvalidPageCountException : DomainException
{
    public InvalidPageCountException(int? pages)
        : base($"Pages must be positive. Got {pages}.") { }
}