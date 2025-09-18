using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Services.GoogleBooks;
using Reveries.Integration.GoogleBooks.Configuration;

namespace Reveries.Integration.GoogleBooks.Extensions;

public static class GoogleBooksServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleBooksServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddGoogleBooks();
        services.AddScoped<IGoogleBookService, GoogleBookService>();
        
        return services;
    }
}