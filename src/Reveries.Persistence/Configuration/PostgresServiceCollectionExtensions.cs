using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Common.Abstractions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Context;
using Reveries.Persistence.Repositories;

namespace Reveries.Persistence.Configuration;

public static class PostgresServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration config)
    {
        DapperConfiguration.Configure();

        var connectionString = config.GetConnectionString("ReveriesDb");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Missing connection string 'ConnectionStrings:ReveriesDb'. Set it via user-secrets (dev) " +
                "or the ConnectionStrings__ReveriesDb environment variable (prod).");

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
