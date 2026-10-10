using Reveries.Application.Common.Exceptions;

namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed class ApplicationExceptionHandler : ProblemDetailsExceptionHandler<AppException>
{
    public ApplicationExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<ApplicationExceptionHandler> logger)
        : base(problemDetailsService, logger)
    {
    }

    protected override ProblemError Map(AppException exception)
    {
        return new ProblemError(
            Status: (int)exception.StatusCode,
            Title: exception.Title,
            ErrorCode: ProblemTypes.ToErrorCode(exception.ErrorType),
            Detail: exception.Message);
    }
}