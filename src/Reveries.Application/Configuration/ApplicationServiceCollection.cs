using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Queries.AllBooks;
using Reveries.Application.Queries.BookByDbId;
using Reveries.Application.Queries.BookByIsbn;
using Reveries.Application.Queries.BookExists;
using Reveries.Application.Queries.BooksByIsbns;
using Reveries.Application.Services;

namespace Reveries.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Commands
        services.AddScoped<CreateBookHandler>();
        services.AddScoped<SetBookSeriesHandler>();

        // Queries
        services.AddScoped<BookByIsbnHandler>();
        services.AddScoped<BooksByIsbnsHandler>();
        services.AddScoped<BookByDbIdHandler>();
        services.AddScoped<AllBooksHandler>();
        services.AddScoped<BookExistsHandler>();
        
        // Services
        services.AddScoped<BookPersistenceService>();
        services.AddScoped<AuthorEnrichmentService>();
        services.AddScoped<BookEnrichmentService>();
        services.AddScoped<BookLookupService>();
        services.AddScoped<AuthorLookupService>();
        services.AddScoped<PublisherLookupService>();
        services.AddScoped<SeriesService>();
        services.AddScoped<BookSeriesService>();
        services.AddScoped<BookReadStatusService>();
        
        return services;
    }
}