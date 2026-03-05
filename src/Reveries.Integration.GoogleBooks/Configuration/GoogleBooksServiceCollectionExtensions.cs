using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Books;
using Reveries.Integration.GoogleBooks.Services;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleBooksServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<GoogleBooksSettings>()
            .Bind(config.GetSection("GoogleBooks"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiUrl), "GoogleBooks: ApiUrl missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.ApiKey), "GoogleBooks: ApiKey missing")
            .ValidateOnStart();
        
        services.AddGoogleBooksClients();
        services.AddScoped<IGoogleBookSearch, GoogleBookSearch>();
        
        return services;
    }
}