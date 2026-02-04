using Microsoft.Extensions.DependencyInjection;
using Reveries.Core.Interfaces;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Persistence.Repositories;
using Reveries.Infrastructure.Postgresql.Services;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class PersistenceServiceCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBookPersistenceService, BookPersistenceService>();
        
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
        services.AddScoped<IBookGenresRepository, BookGenresRepository>();
        services.AddScoped<IDeweyDecimalRepository, DeweyDecimalRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();

        return services;
    }
}