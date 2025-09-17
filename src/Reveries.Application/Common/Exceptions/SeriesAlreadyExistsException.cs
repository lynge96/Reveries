namespace Reveries.Application.Common.Exceptions;

public class SeriesAlreadyExistsException : Exception
{
    public SeriesAlreadyExistsException(string message) : base(message)
    {
    }
}