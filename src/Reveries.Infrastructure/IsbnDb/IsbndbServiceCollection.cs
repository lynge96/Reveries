using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Isbndb;

namespace Reveries.Infrastructure.IsbnDb;

public static class IsbndbServiceCollection
{
    public static IServiceCollection AddIsbndb(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = Environment.GetEnvironmentVariable("ISBNDB_API_KEY") ?? string.Empty;
        var apiUrl = Environment.GetEnvironmentVariable("ISBNDB_API_URL") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(apiKey))
            throw new InvalidOperationException("ISBNDB API key is missing");

        if (string.IsNullOrWhiteSpace(apiUrl))
            throw new InvalidOperationException("ISBNDB API URL is missing");

        services.AddHttpClient("Isbndb", client =>
        {
            client.BaseAddress = new Uri(apiUrl);
            client.DefaultRequestHeaders.Add("Authorization", apiKey);
        });

        services.AddTransient<IIsbndbBookClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbBookClient(factory.CreateClient("Isbndb"));
        });

        services.AddTransient<IIsbndbAuthorClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbAuthorClient(factory.CreateClient("Isbndb"));
        });

        services.AddTransient<IIsbndbPublisherClient>(provider =>
        {
            var factory = provider.GetRequiredService<IHttpClientFactory>();
            return new IsbndbPublisherClient(factory.CreateClient("Isbndb"));
        });

        return services;
    }
}