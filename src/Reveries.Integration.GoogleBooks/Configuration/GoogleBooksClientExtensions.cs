using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Core.Configuration;
using Reveries.Integration.GoogleBooks.Clients;
using Reveries.Integration.GoogleBooks.Interfaces;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksClientExtensions
{
    public static IServiceCollection AddGoogleBooksClients(this IServiceCollection services)
    {
        services.AddHttpClient<IGoogleBooksClient, GoogleBooksClient>(ConfigureGoogleBooksClient);

        return services;
    }
    
    private static void ConfigureGoogleBooksClient(IServiceProvider serviceProvider, HttpClient client)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<GoogleBooksSettings>>().Value;

        client.BaseAddress = new Uri(settings.ApiUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    }
}