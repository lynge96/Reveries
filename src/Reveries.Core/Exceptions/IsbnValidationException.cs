using System.Net;

namespace Reveries.Core.Exceptions;

public class IsbnValidationException : BaseAppException
{
    public IsbnValidationException(string message) : 
        base(message, HttpStatusCode.BadRequest) 
    { }
}
