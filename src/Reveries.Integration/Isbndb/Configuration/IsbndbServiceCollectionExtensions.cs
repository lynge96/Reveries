using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Books.Interfaces;
using Reveries.Integration.Isbndb.Services;

namespace Reveries.Integration.Isbndb.Configuration;

public static class IsbndbServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndb(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<IsbndbSettings>()
            .Bind(config.GetSection(IsbndbSettings.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "Isbndb: ApiUrl missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "Isbndb: ApiKey missing")
            .Validate(s => s.MaxBulkIsbns > 0, "Isbndb: MaxBulkIsbns must be positive")
            .ValidateOnStart();

        services.AddIsbndbClients();

        services.AddScoped<IBookSearch, IsbndbBookService>();

        return services;
    }
}