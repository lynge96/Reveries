namespace Reveries.Api.Configuration.ExceptionHandling;

public sealed record ProblemError(int Status, string Title, string ErrorCode, string Detail);