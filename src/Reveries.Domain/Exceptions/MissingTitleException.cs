namespace Reveries.Domain;

public sealed class MissingTitleException : DomainException
{
    public MissingTitleException(string? bookTitle)
        : base($"Book title is missing, it cannot be empty: {bookTitle}") { }
}
