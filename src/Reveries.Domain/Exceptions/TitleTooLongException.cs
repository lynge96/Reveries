namespace Reveries.Domain.Exceptions;

public sealed class TitleTooLongException : DomainException
{
    public int Length { get; }
    public int MaxLength { get; }

    public TitleTooLongException(int length, int maxLength)
        : base($"Title cannot exceed {maxLength} characters, but was {length}.")
    {
        Length = length;
        MaxLength = maxLength;
    }
}