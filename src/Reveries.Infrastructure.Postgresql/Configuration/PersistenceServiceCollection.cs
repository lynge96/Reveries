using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Services;
using Reveries.Application.Services;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Infrastructure.Postgresql.Persistence.Repositories;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class PersistenceServiceCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBookPersistenceService, BookPersistenceService>();
        services.AddScoped<IBookSeriesService, BookSeriesService>();
        
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
        services.AddScoped<IBookGenresRepository, BookGenresRepository>();
        services.AddScoped<IDeweyDecimalsRepository, DeweyDecimalsRepository>();
        services.AddScoped<IBookDeweyDecimalsRepository, BookDeweyDecimalsRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();

        return services;
    }
}