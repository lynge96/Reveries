using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Integration.Isbndb.Services;

namespace Reveries.Integration.Isbndb.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIsbndbServices(this IServiceCollection services)
    {
        services.AddIsbndbClients();

        services.AddScoped<IIsbndbBookService, IsbndbBookService>();
        services.AddScoped<IIsbndbAuthorService, IsbndbAuthorService>();
        services.AddScoped<IIsbndbPublisherService, IsbndbPublisherService>();
        
        return services;
    }
}
