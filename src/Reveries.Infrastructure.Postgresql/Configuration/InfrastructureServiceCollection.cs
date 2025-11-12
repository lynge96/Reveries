using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Configuration;
using Reveries.Core.Interfaces.Persistence;
using Reveries.Infrastructure.Postgresql.Persistence;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddRepositories();

        services.AddSingleton<NpgsqlDataSource>(serviceProvider =>
        {
            var settings = serviceProvider.GetRequiredService<IOptions<PostgresSettings>>().Value;
            var connectionString = settings.GetConnectionString();

            var builder = new NpgsqlDataSourceBuilder(connectionString);
            
            var env = serviceProvider.GetRequiredService<IHostEnvironment>();
            if (env.IsDevelopment())
            {
                builder.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>()); // log SQL i dev
                builder.EnableParameterLogging();
            }

            return builder.Build();
        });
        
        services.AddScoped<IDbContext, PostgresDbContext>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        return services;
    }
}