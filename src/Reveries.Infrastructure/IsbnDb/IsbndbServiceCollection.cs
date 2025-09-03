using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Settings;

namespace Reveries.Infrastructure.IsbnDb;

public static class IsbndbServiceCollection
{
    public static IServiceCollection AddIsbndb(this IServiceCollection services, IConfiguration configuration)
    {
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