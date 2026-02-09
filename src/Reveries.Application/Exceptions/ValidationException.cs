using System.Net;
using FluentValidation.Results;

namespace Reveries.Application.Exceptions;

public class ValidationException : ApplicationException
{
    public IEnumerable<ValidationFailure> Failures { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
    {
        Failures = failures;
    }
    
    public ValidationException(string message) : base(message, HttpStatusCode.BadRequest) { }
}