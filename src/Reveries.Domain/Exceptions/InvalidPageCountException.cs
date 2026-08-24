namespace Reveries.Domain.Exceptions;

public sealed class InvalidPageCountException : DomainException
{
    public int? Pages { get; }

    public InvalidPageCountException(int? pages)
        : base($"Pages must be positive. Got {pages}.")
    {
        Pages = pages;
    }
}