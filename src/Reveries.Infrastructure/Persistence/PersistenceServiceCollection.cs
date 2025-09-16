using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Persistence.Context;
using Reveries.Infrastructure.Persistence.Interfaces;
using Reveries.Infrastructure.Persistence.Repositories;

namespace Reveries.Infrastructure.Persistence;

public static class PersistenceServiceCollection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<IPostgresDbContext, PostgresDbContext>();

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IBookAuthorsRepository, BookAuthorsRepository>();
        services.AddScoped<IBookSubjectsRepository, BookSubjectsRepository>();
        services.AddScoped<IBookDimensionsRepository, BookDimensionsRepository>();
        services.AddScoped<IDeweyDecimalRepository, DeweyDecimalRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        return services;
    }
}