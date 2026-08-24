namespace Reveries.Domain.Exceptions;

public sealed class InvalidSeriesNumberException : DomainException
{
    public int? SeriesNumber { get; }

    public InvalidSeriesNumberException(int? seriesNumber)
        : base($"Invalid series number: {seriesNumber} - series number must be positive")
    {
        SeriesNumber = seriesNumber;
    }
}