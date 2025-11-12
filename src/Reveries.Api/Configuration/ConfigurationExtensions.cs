using DotNetEnv;
using Reveries.Core.Configuration;

namespace Reveries.Api.Configuration;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddAppConfiguration(this IServiceCollection services, IConfiguration config, IHostEnvironment env)
    {
        // === External APIs ===
        services.AddOptions<IsbndbSettings>()
            .Bind(config.GetSection("ExternalApis:Isbndb"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "Isbndb: ApiUrl missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "Isbndb: ApiKey missing")
            .ValidateOnStart();

        services.AddOptions<GoogleBooksSettings>()
            .Bind(config.GetSection("ExternalApis:GoogleBooks"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "GoogleBooks: ApiUrl missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "GoogleBooks: ApiKey missing")
            .ValidateOnStart();

        // === Postgres ===
        services.AddOptions<PostgresSettings>()
            .Bind(config.GetSection("Postgres"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Host), "Postgres: Host missing")
            .Validate(s => s.Port > 0, "Postgres: Port invalid")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Database), "Postgres: Database missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Username), "Postgres: Username missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Password) || !string.IsNullOrWhiteSpace(s.ConnectionString), "Postgres: Password or ConnectionString required")
            .ValidateOnStart();

        // === Redis ===
        services.AddOptions<RedisSettings>()
            .Bind(config.GetSection("Redis"))
            .ValidateOnStart();

        return services;
    }
}