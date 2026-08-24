namespace Reveries.Domain.Exceptions;

public sealed class MissingTitleException : DomainException
{
    public string? ProvidedTitle { get; }

    public MissingTitleException(string? providedTitle)
        : base($"Work title is missing, it cannot be empty: {providedTitle}")
    {
        ProvidedTitle = providedTitle;
    }
}