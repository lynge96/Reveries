using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Services.Isbndb;
using Reveries.Integration.Isbndb.Configuration;

namespace Reveries.Integration.Isbndb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.ConfigureIsbndbSettings();
        services.AddScoped<IIsbndbBookService, IsbndbBookService>();
        
        return services;
    }
}
