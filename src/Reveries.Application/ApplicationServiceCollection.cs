using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Authors.Services;
using Reveries.Application.Books.Commands.CreateBook;
using Reveries.Application.Books.Commands.SetBookSeries;
using Reveries.Application.Books.Queries.GetAllBooks;
using Reveries.Application.Books.Queries.GetBookByDbId;
using Reveries.Application.Books.Queries.GetBookByIsbn;
using Reveries.Application.Books.Queries.GetBookExists;
using Reveries.Application.Books.Queries.GetBooksByIsbns;
using Reveries.Application.Books.Services;
using Reveries.Application.Publishers.Services;
using Reveries.Application.Services.BookSeries;

namespace Reveries.Application;

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
        services.AddScoped<GetBookExistsHandler>();
        
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