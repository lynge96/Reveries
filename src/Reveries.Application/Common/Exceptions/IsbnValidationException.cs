namespace Reveries.Application.Common.Exceptions;

public class IsbnValidationException : ArgumentException
{
    public IsbnValidationException(string message) 
        : base(message) { }
        
    public IsbnValidationException(string message, string paramName) 
        : base(message, paramName) { }
}
