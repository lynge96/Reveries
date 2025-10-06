using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Infrastructure.Postgresql.Persistence;
using Reveries.Infrastructure.Postgresql.Persistence.Repositories;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class PersistenceServiceCollection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<IDbContext, PostgresDbContext>();

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