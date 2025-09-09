using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Core.Entities.Settings;
using Reveries.Integration.GoogleBooks.Clients;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksServiceCollection
{
    public static IServiceCollection AddGoogleBooks(this IServiceCollection services)
    {
        services.AddHttpClient("GoogleBooks", (provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<GoogleBooksSettings>>().Value;
            
            client.BaseAddress = new Uri(settings.ApiUrl);
        });
        
        services.AddTransient<IGoogleBooksClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("GoogleBooks");
            var options = provider.GetRequiredService<IOptions<GoogleBooksSettings>>();

            return new GoogleBooksClient(httpClient, options);
        });

        return services;
    }
}