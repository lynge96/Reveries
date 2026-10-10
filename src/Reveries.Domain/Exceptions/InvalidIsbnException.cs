namespace Reveries.Domain.Exceptions;

public sealed class InvalidIsbnException : DomainException
{
    public string? AttemptedValue { get; }

    private InvalidIsbnException(string message, string? attemptedValue)
        : base("Invalid ISBN", message)
    {
        AttemptedValue = attemptedValue;
    }

    public static InvalidIsbnException Empty() =>
        new("An ISBN is required, but none was provided.", null);

    public static InvalidIsbnException InvalidChecksum(string isbn) =>
        new($"The check digit of ISBN '{isbn}' is invalid.", isbn);

    public static InvalidIsbnException InvalidLength(string isbn) =>
        new($"An ISBN must be 10 or 13 characters long, but '{isbn}' has {isbn.Length}.", isbn);
}