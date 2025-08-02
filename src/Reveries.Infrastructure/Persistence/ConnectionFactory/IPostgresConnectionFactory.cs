using Npgsql;

namespace Reveries.Infrastructure.Persistence.ConnectionFactory;

public interface IPostgresConnectionFactory
{
    Task<NpgsqlConnection> CreateConnectionAsync();
}