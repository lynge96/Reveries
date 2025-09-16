using Npgsql;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Interfaces;

public interface IPostgresDbContext : IDbContext
{
    Task<NpgsqlConnection> GetConnectionAsync();
}
