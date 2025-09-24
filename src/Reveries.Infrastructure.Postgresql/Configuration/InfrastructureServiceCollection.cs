using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Infrastructure.Postgresql.Persistence;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresSettings>(options =>
        {
            options.Host = Environment.GetEnvironmentVariable("DB_HOST") 
                               ?? throw new InvalidOperationException("DB_HOST missing");
            options.Database = Environment.GetEnvironmentVariable("POSTGRES_DB") 
                               ?? throw new InvalidOperationException("POSTGRES_DB missing");
            options.Username = Environment.GetEnvironmentVariable("POSTGRES_USER") 
                               ?? throw new InvalidOperationException("POSTGRES_USER missing");
            options.Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") 
                               ?? throw new InvalidOperationException("POSTGRES_PASSWORD missing");
        });
        
        services.AddPersistence();
        
        return services;
    }

}