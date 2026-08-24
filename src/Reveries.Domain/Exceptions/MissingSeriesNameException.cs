namespace Reveries.Domain.Exceptions;

public sealed class MissingSeriesNameException : DomainException
{
    public string? ProvidedName { get; }

    public MissingSeriesNameException(string? providedName)
        : base($"Series name is missing, it cannot be empty: {providedName}")
    {
        ProvidedName = providedName;
    }
}