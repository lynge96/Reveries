using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Queries.GetAllBooks;
using Reveries.Application.Queries.GetBookByDbId;
using Reveries.Application.Queries.GetBookByIsbn;
using Reveries.Application.Queries.GetBookByIsbns;
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
        services.AddScoped<GetBookByIsbnHandler>();
        services.AddScoped<GetBooksByIsbnsHandler>();
        services.AddScoped<GetBookByDbIdHandler>();
        services.AddScoped<GetAllBooksHandler>();
        
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