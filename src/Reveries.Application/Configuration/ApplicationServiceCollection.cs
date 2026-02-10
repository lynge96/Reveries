using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Queries.GetBookByIsbn;
using Reveries.Application.Services;
using Reveries.Core.Models;

namespace Reveries.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ICommandHandler<CreateBookCommand, int>, CreateBookCommandHandler>();
        services.AddScoped<ICommandHandler<SetBookSeriesCommand, int>, SetBookSeriesCommandHandler>();

        services.AddScoped<IQueryHandler<GetBookByIsbnQuery, Book>, GetBookByIsbnHandler>();
        
        services.AddScoped<IAuthorEnrichmentService, AuthorEnrichmentService>();
        services.AddScoped<IBookEnrichmentService, BookEnrichmentService>();
        
        services.AddScoped<IBookLookupService, BookLookupService>();
        services.AddScoped<IAuthorLookupService, AuthorLookupService>();
        services.AddScoped<IPublisherLookupService, PublisherLookupService>();
        services.AddScoped<ISeriesService, SeriesService>();
        
        return services;
    }
}