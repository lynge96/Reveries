using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Services;
using Reveries.Application.Publishers.Services;
using Reveries.Application.Services.BookSeries;

namespace Reveries.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        
        // Services
        services.AddScoped<BookPersistenceService>();
        services.AddScoped<AuthorEnrichmentService>();
        services.AddScoped<BookEnrichmentService>();
        services.AddScoped<BookLookupService>();
        services.AddScoped<AuthorLookupService>();
        services.AddScoped<PublisherLookupService>();
        services.AddScoped<CreateSeriesService>();
        services.AddScoped<BookSeriesService>();
        services.AddScoped<BookReadStatusService>();
        
        return services;
    }
}