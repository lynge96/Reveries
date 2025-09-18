using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.GoogleBooks;
using Reveries.Application.Interfaces.Isbndb;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Services.GoogleBooks;
using Reveries.Application.Services.Isbndb;

namespace Reveries.Application.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Isbndb services
        services.AddScoped<IIsbndbBookService, IsbndbBookService>();
        services.AddScoped<IIsbndbAuthorService, IsbndbAuthorService>();
        services.AddScoped<IIsbndbPublisherService, IsbndbPublisherService>();
        // Google Books services
        services.AddScoped<IGoogleBookService, GoogleBookService>();
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