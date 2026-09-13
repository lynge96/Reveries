using Microsoft.Extensions.DependencyInjection;

namespace Reveries.Persistence.Migrations;

public static class MigrationServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseMigrations(this IServiceCollection services)
    {
        services.AddHostedService<DatabaseMigrationHostedService>();

        return services;
    }
}