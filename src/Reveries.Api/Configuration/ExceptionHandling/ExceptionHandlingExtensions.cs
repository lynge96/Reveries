namespace Reveries.Api.Configuration.ExceptionHandling;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services, IHostEnvironment environment)
    {
        services.AddExceptionHandler<ExternalDependencyExceptionHandler>();
        services.AddExceptionHandler<ApplicationExceptionHandler>();
        services.AddExceptionHandler<DomainExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

                if (environment.IsDevelopment() && context.Exception is not null)
                {
                    context.ProblemDetails.Extensions["stackTrace"] = context.Exception.StackTrace;
                    context.ProblemDetails.Extensions["innerException"] = context.Exception.InnerException?.Message;
                }
            };
        });

        return services;
    }
}