namespace Reveries.Domain.Exceptions;

public sealed class MissingTitleException : DomainException
{
    public MissingTitleException(string? workTitle)
        : base($"Work title is missing, it cannot be empty: {workTitle}") { }
}
