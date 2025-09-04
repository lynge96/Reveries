using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Services.Isbndb;

namespace Reveries.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IIsbndbBookService, IsbndbBookService>();
        services.AddScoped<IIsbndbAuthorService, IsbndbAuthorService>();
        services.AddScoped<IIsbndbPublisherService, IsbndbPublisherService>();
        services.AddScoped<IBookManagementService, BookManagementService>();
        services.AddScoped<IBookEnrichmentService, BookEnrichmentService>();
        
        return services;
    }
}