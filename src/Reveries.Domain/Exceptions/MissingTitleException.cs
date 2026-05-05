namespace Reveries.Core.Exceptions;

public sealed class MissingTitleException : DomainException
{
    public MissingTitleException(string? bookTitle)
        : base($"Book title is missing, it cannot be empty: {bookTitle}") { }
}