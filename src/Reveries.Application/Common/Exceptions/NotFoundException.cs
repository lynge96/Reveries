using System.Net;

namespace Reveries.Application.Common.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound) 
    { }
}