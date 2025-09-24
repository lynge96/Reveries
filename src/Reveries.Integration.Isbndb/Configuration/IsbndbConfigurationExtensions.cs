using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Integration.Isbndb.Clients;
using Reveries.Integration.Isbndb.Clients.Interfaces;

namespace Reveries.Integration.Isbndb.Configuration;

public static class IsbndbConfigurationExtensions
{
    public static IServiceCollection ConfigureIsbndbSettings(this IServiceCollection services)
    {
        services.Configure<IsbndbSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("ISBNDB_API_KEY") 
                             ?? throw new InvalidOperationException("ISBNDB_API_KEY missing");
            options.ApiUrl = Environment.GetEnvironmentVariable("ISBNDB_API_URL") 
                             ?? throw new InvalidOperationException("ISBNDB_API_URL missing");
        });
        
        services.AddHttpClient("Isbndb", (provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<IsbndbSettings>>().Value;
            
            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("Authorization", settings.ApiKey);
        });
        
        services.AddTransient<IIsbndbBookClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbBookClient(httpClientFactory.CreateClient("Isbndb"));
        });

        services.AddTransient<IIsbndbAuthorClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbAuthorClient(httpClientFactory.CreateClient("Isbndb"));
        });

        services.AddTransient<IIsbndbPublisherClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbPublisherClient(httpClientFactory.CreateClient("Isbndb"));
        });
        
        return services;
    }
}