using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Reveries.Infrastructure.Postgresql.Configuration;

public class PostgresHealthCheck : IHealthCheck
{
    private readonly NpgsqlDataSource _dataSource;
    
    public PostgresHealthCheck(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("PostgreSQL connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "PostgreSQL connection failed",
                exception: ex);
        }
    }
}