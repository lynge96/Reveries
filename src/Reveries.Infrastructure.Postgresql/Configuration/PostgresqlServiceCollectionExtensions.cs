using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Persistence;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class PostgresqlServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresql(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<PostgresSettings>()
            .Bind(config.GetSection("Postgres"))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Host), "Postgres: Host missing")
            .Validate(s => s.Port > 0, "Postgres: Port invalid")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Database), "Postgres: Database missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Username), "Postgres: Username missing")
            .Validate(s => !string.IsNullOrWhiteSpace(s.Password) || !string.IsNullOrWhiteSpace(s.ConnectionString), "Postgres: Password or ConnectionString required")
            .ValidateOnStart();
        
        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<PostgresSettings>>().Value;
            var connectionString = settings.GetConnectionString();

            var builder = new NpgsqlDataSourceBuilder(connectionString);
            
            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment()) 
            {
                // log SQL i dev
                builder.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>()); 
                builder.EnableParameterLogging();
            }

            return builder.Build();
        });
        
        services.AddRepositories();
        
        services.AddScoped<IDbContext, PostgresDbContext>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
}