using Reveries.Domain.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class DomainExceptionHandler : ProblemDetailsExceptionHandler<DomainException>
{
    public DomainExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<DomainExceptionHandler> logger)
        : base(problemDetailsService, logger)
    {
    }

    protected override ProblemError Map(DomainException exception)
    {
        return new ProblemError(
            Status: StatusCodes.Status400BadRequest,
            Title: "Domain Validation Error",
            ErrorCode: ProblemTypes.ToErrorCode(exception.ErrorType),
            Detail: exception.Message);
    }
}