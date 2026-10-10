namespace Reveries.Domain.Exceptions;

public sealed class MissingTitleException : DomainException
{
    public string? ProvidedTitle { get; }

    public MissingTitleException(string? providedTitle)
        : base("Missing Title", "A work title is required, but the provided value was empty or whitespace.")
    {
        ProvidedTitle = providedTitle;
    }
}