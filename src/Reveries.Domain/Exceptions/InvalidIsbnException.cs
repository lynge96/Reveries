namespace Reveries.Domain.Exceptions;

public sealed class InvalidIsbnException : DomainException
{
    public string? AttemptedValue { get; }

    private InvalidIsbnException(string message, string? attemptedValue)
        : base(message)
    {
        AttemptedValue = attemptedValue;
    }

    public static InvalidIsbnException Empty() =>
        new("ISBN cannot be null or empty.", null);

    public static InvalidIsbnException InvalidChecksum(string isbn) =>
        new($"Invalid ISBN-{isbn.Length} checksum: {isbn}.", isbn);

    public static InvalidIsbnException InvalidLength(string isbn) =>
        new($"ISBN must be either 10 or 13 characters long: {isbn}.", isbn);
}