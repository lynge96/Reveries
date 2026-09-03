using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class InvalidRequestException : ApplicationException
{
    public InvalidRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    { }
}