using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Services;

namespace Reveries.Application.Services.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IBookManagementService, BookManagementService>();
        services.AddScoped<IBookEnrichmentService, BookEnrichmentService>();
        services.AddScoped<IBookLookupService, BookLookupService>();
        services.AddScoped<IAuthorLookupService, AuthorLookupService>();
        services.AddScoped<IPublisherLookupService, PublisherLookupService>();
        services.AddScoped<IBookSeriesService, BookSeriesService>();
        
        return services;
    }
}