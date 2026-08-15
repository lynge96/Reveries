using Dapper;
using Reveries.Persistence.Interfaces;

namespace Reveries.Persistence.Context;

public static class DbContextCommandExtensions
{
    public static CommandDefinition CreateCommand(
        this IDbContext dbContext,
        string sql,
        object? parameters = null,
        CancellationToken ct = default) =>
        new(sql, parameters, dbContext.CurrentTransaction, cancellationToken: ct);
}