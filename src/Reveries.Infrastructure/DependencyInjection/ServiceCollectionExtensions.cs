using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Core.Settings;
using Reveries.Infrastructure.ISBNDB;

namespace Reveries.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IsbndbSettings>(configuration.GetSection("Isbndb"));
        services.AddIsbndbClients();
        return services;
    }
    
    private static IServiceCollection AddIsbndbClients(this IServiceCollection services)
    {
        services.AddHttpClient("Isbndb", (provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<IsbndbSettings>>().Value;

            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("Authorization", settings.ApiKey);
        });
        
        services.AddTransient<IIsbndbBookClient, IsbndbBookClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbBookClient(httpClientFactory.CreateClient("Isbndb"));
        });
        
        services.AddTransient<IIsbndbAuthorClient, IsbndbAuthorClient>(provider =>
        {
            var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbAuthorClient(httpClientFactory.CreateClient("Isbndb"));
        });

        return services;
    }
}