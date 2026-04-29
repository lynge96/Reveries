namespace Reveries.Api.Configuration.Cors;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Development", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("https://scanner.reveries.dk", "https://localhost:7238")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }
}