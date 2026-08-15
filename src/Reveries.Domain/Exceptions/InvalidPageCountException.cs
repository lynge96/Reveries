namespace Reveries.Domain;

public sealed class InvalidPageCountException : DomainException
{
    public InvalidPageCountException(int? pages)
        : base($"Pages must be positive. Got {pages}.") { }
}
