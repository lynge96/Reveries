using Npgsql;
using Reveries.Core.Interfaces;
using Reveries.Core.Interfaces.Persistence;

namespace Reveries.Infrastructure.Interfaces.Persistence;

public interface IPostgresDbContext : IDbContext
{
    Task<NpgsqlConnection> GetConnectionAsync();
}
