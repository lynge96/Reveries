using Reveries.Application.Interfaces;
using Reveries.Infrastructure.ISBNDB;
using Microsoft.Extensions.DependencyInjection;

namespace Reveries.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbClient(this IServiceCollection services, string apiKey)
    {
        services.AddHttpClient<IIsbndbClient, IsbndbClient>(client =>
        {
            client.BaseAddress = new Uri("https://api2.isbndb.com");
            client.DefaultRequestHeaders.Add("Authorization", apiKey);
        });

        return services;
    }
}