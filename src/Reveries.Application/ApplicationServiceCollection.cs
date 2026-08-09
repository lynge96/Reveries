using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Authors.Interfaces;
using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Services;
using Reveries.Application.BookSeries.Interfaces;
using Reveries.Application.BookSeries.Services;
using Reveries.Application.Publishers.Interfaces;
using Reveries.Application.Publishers.Services;

namespace Reveries.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        
        // Services
        // Books
        services.AddScoped<IBookPersistenceService, BookPersistenceService>();
        services.AddScoped<IBookMergerService, BookMergerService>();
        services.AddScoped<IBookLookupService, BookLookupService>();
        services.AddScoped<IBookReadStatusService, BookReadStatusService>();
        
        // Authors
        services.AddScoped<IAuthorEnrichmentService, AuthorEnrichmentService>();
        services.AddScoped<IAuthorLookupService, AuthorLookupService>();
        
        // Publishers
        services.AddScoped<IPublisherLookupService, PublisherLookupService>();
        
        // Series
        services.AddScoped<ICreateSeriesService, CreateSeriesService>();
        services.AddScoped<IBookSeriesService, BookSeriesService>();
        
        return services;
    }
}