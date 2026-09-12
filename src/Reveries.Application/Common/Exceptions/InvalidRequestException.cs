using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class InvalidRequestException : AppException
{
    public InvalidRequestException(string message)
        : base(message, HttpStatusCode.BadRequest)
    { }
}