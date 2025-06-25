using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces;
using Reveries.Core.Settings;
using Reveries.Infrastructure.ISBNDB;

namespace Reveries.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IsbndbSettings>(configuration.GetSection("Isbndb"));
        services.AddIsbndbClient();
        return services;
    }
    
    private static IServiceCollection AddIsbndbClient(this IServiceCollection services)
    {
        services.AddHttpClient<IIsbndbClient, IsbndbClient>((provider, client) =>
        {
            var settings = provider.GetRequiredService<IOptions<IsbndbSettings>>().Value;

            client.BaseAddress = new Uri(settings.ApiUrl);
            client.DefaultRequestHeaders.Add("Authorization", settings.ApiKey);
        });

        return services;
    }
}