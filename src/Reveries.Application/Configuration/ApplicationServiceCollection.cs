using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Commands;
using Reveries.Application.Commands.CreateBook;
using Reveries.Application.Commands.SetBookSeries;
using Reveries.Application.Interfaces.Messaging;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Services;

namespace Reveries.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ICommandHandler<CreateBookCommand>, CreateBookCommandHandler>();
        services.AddScoped<ICommandHandler<SetBookSeriesCommand>, SetBookSeriesCommandHandler>();
        
        services.AddScoped<IAuthorEnrichmentService, AuthorEnrichmentService>();
        services.AddScoped<IBookEnrichmentService, BookEnrichmentService>();
        
        services.AddScoped<IBookLookupService, BookLookupService>();
        services.AddScoped<IAuthorLookupService, AuthorLookupService>();
        services.AddScoped<IPublisherLookupService, PublisherLookupService>();
        services.AddScoped<ISeriesService, SeriesService>();
        
        return services;
    }
}