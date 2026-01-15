using System.Net;
using FluentValidation.Results;

namespace Reveries.Core.Exceptions;

public class ValidationException : BaseAppException
{
    public IEnumerable<ValidationFailure> Failures { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.", HttpStatusCode.BadRequest)
    {
        Failures = failures;
    }
}