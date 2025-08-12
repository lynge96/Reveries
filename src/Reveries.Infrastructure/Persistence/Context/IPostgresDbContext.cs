using Npgsql;
using Reveries.Core.Interfaces;

namespace Reveries.Infrastructure.Persistence.Context;

public interface IPostgresDbContext : IDbContext
{
    Task<NpgsqlConnection> GetConnectionAsync();
}
