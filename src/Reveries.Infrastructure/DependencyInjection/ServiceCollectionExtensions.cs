using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Application.Interfaces;
using Reveries.Infrastructure.ISBNDB;

namespace Reveries.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbClient(this IServiceCollection services)
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