using Microsoft.Extensions.DependencyInjection;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Infrastructure.Postgresql.Persistence.Repositories;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class PersistenceServiceCollection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
        services.AddScoped<IBookSubjectsRepository, BookSubjectsRepository>();
        services.AddScoped<IBookDimensionsRepository, BookDimensionsRepository>();
        services.AddScoped<IDeweyDecimalRepository, DeweyDecimalRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();

        return services;
    }
}