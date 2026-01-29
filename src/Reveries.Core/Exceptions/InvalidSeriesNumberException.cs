namespace Reveries.Core.Exceptions;

public sealed class InvalidSeriesNumberException : DomainException
{
    public InvalidSeriesNumberException(int? seriesNumber)
        : base($"Invalid series number: {seriesNumber} - series number must be positive") { }
}