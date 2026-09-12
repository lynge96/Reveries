using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Books.Interfaces;
using Reveries.Integration.GoogleBooks.Services;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleBooks(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<GoogleBooksSettings>()
            .Bind(config.GetSection(GoogleBooksSettings.SectionName))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "GoogleBooks: ApiUrl missing")
            .ValidateOnStart();

        services.AddGoogleBooksClients();
        services.AddScoped<IBookSearch, GoogleBooksSource>();

        return services;
    }
}
