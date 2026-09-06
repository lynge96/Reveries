using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Context;
using Reveries.Persistence.Exceptions;
using Reveries.Persistence.Repositories;

namespace Reveries.Persistence.Configuration;

public static class PostgresServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration config)
    {
        DapperConfiguration.Configure();

        var connectionString = config.GetConnectionString("ReveriesDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new MissingConnectionStringException("ReveriesDb");

        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var builder = new NpgsqlDataSourceBuilder(connectionString);

            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                builder.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
                builder.EnableParameterLogging();
            }

            return builder.Build();
        });

        services.AddSingleton<ResiliencePipeline>(serviceProvider =>
            DbResiliencePipeline.Build(
                serviceProvider.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Reveries.Persistence.Resilience")));

        // Entity tabeller
        services.AddScoped<IWorkRepository, WorkRepository>();
        services.AddScoped<IEditionRepository, EditionRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IDeweyDecimalsRepository, DeweyDecimalsRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
        services.AddScoped<IBookQueryRepository, BookQueryRepository>();

        // DbContext
        services.AddScoped<IDbContext, PostgresDbContext>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }

    public static IHealthChecksBuilder AddPostgresHealthCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddNpgSql(
            sp => sp.GetRequiredService<NpgsqlDataSource>(),
            name: "postgres");
    }
}
