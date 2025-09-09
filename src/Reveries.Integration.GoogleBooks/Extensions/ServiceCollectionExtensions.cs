using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Services.GoogleBooks;
using Reveries.Core.Entities.Settings;
using Reveries.Integration.GoogleBooks.Configuration;

namespace Reveries.Integration.GoogleBooks.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleBooksServices(this IServiceCollection services)
    {
        services.Configure<GoogleBooksSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_KEY") 
                             ?? throw new InvalidOperationException("GOOGLE_BOOKS_API_KEY missing");
            options.ApiUrl = Environment.GetEnvironmentVariable("GOOGLE_BOOKS_API_URL") 
                             ?? throw new InvalidOperationException("GOOGLE_BOOKS_API_URL missing");
        });
        
        services.AddHttpClient();
        services.AddGoogleBooks();
        services.AddScoped<IGoogleBookService, GoogleBookService>();
        
        return services;
    }
}