using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Integration.GoogleBooks.Clients;
using Reveries.Integration.GoogleBooks.Interfaces;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksConfigurationExtensions
{
    public static IServiceCollection AddGoogleBooks(this IServiceCollection services)
    {
        services.Configure<GoogleBooksSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_KEY") 
                             ?? throw new InvalidOperationException("GOOGLE_BOOKS_API_KEY missing");
            options.ApiUrl = Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_URL") 
                             ?? throw new InvalidOperationException("GOOGLE_BOOKS_API_URL missing");
        });
        
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