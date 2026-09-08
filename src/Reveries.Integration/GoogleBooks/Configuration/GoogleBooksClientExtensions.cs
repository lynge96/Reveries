using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Integration.GoogleBooks.Clients;
using Reveries.Integration.GoogleBooks.Interfaces;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksClientExtensions
{
    internal static IServiceCollection AddGoogleBooksClients(this IServiceCollection services)
    {
        services.AddHttpClient<IGoogleBooksClient, GoogleBooksClient>(ConfigureGoogleBooksClient)
            .AddStandardResilienceHandler();

        return services;
    }

    private static void ConfigureGoogleBooksClient(IServiceProvider serviceProvider, HttpClient client)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<GoogleBooksSettings>>().Value;

        client.BaseAddress = new Uri(settings.ApiUrl);
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Reveries/1.0");
    }
}
