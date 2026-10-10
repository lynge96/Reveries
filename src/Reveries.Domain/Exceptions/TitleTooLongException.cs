namespace Reveries.Domain.Exceptions;

public sealed class TitleTooLongException : DomainException
{
    public int Length { get; }
    public int MaxLength { get; }

    public TitleTooLongException(int length, int maxLength)
        : base("Title Too Long", $"A work title must be at most {maxLength} characters, but the provided title has {length}.")
    {
        Length = length;
        MaxLength = maxLength;
    }
}