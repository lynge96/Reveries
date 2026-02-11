using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Commands;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Queries;
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
        services.AddScoped<IAuthorEnrichmentService, AuthorEnrichmentService>();
        services.AddScoped<IBookEnrichmentService, BookEnrichmentService>();
        services.AddScoped<IBookLookupService, BookLookupService>();
        services.AddScoped<IAuthorLookupService, AuthorLookupService>();
        services.AddScoped<IPublisherLookupService, PublisherLookupService>();
        services.AddScoped<ISeriesService, SeriesService>();
        services.AddScoped<IBookSeriesService, BookSeriesService>();
        services.AddScoped<IBookReadStatusService, BookReadStatusService>();
        
        return services;
    }
}