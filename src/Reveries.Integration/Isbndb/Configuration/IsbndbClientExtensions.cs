using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Reveries.Integration.Isbndb.Clients;
using Reveries.Integration.Isbndb.Interfaces;

namespace Reveries.Integration.Isbndb.Configuration;

public static class IsbndbClientExtensions
{
    internal static IServiceCollection AddIsbndbClients(this IServiceCollection services)
    {
        services.AddHttpClient<IIsbndbBookClient, IsbndbBookClient>(ConfigureIsbndb)
            .AddStandardResilienceHandler();

        return services;
    }

    private static void ConfigureIsbndb(IServiceProvider serviceProvider, HttpClient client)
    {
        var settings = serviceProvider.GetRequiredService<IOptions<IsbndbSettings>>().Value;
        client.BaseAddress = new Uri(settings.ApiUrl);
        client.Timeout = Timeout.InfiniteTimeSpan;
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Reveries/1.0");
        client.DefaultRequestHeaders.Add("Authorization", settings.ApiKey);
    }
}
