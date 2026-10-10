using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class InvalidRequestException : AppException
{
    public InvalidRequestException(string message)
        : base("Invalid Request", message, HttpStatusCode.BadRequest)
    { }
}