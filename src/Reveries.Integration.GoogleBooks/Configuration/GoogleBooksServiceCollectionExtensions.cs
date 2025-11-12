using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Integration.GoogleBooks.Services;

namespace Reveries.Integration.GoogleBooks.Configuration;

public static class GoogleBooksServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleBooksServices(this IServiceCollection services)
    {
        services.AddGoogleBooksClients();
        services.AddScoped<IGoogleBookService, GoogleBookService>();
        
        return services;
    }
}