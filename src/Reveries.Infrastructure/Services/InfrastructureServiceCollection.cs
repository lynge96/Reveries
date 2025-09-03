using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reveries.Core.Settings;
using Reveries.Infrastructure.IsbnDb;
using Reveries.Infrastructure.Persistence;

namespace Reveries.Infrastructure.Services;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IsbndbSettings>(options =>
        {
            options.ApiKey = Environment.GetEnvironmentVariable("ISBNDB_API_KEY") 
                             ?? throw new InvalidOperationException("ISBNDB_API_KEY missing");
            options.ApiUrl = Environment.GetEnvironmentVariable("ISBNDB_API_URL") 
                             ?? throw new InvalidOperationException("ISBNDB_API_URL missing");
        });
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
        
        services.AddIsbndb(configuration);
        services.AddPersistence();
        
        return services;
    }

}