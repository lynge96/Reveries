using Dapper;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IPostgresDbContext _dbContext;

    public SeriesRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetOrCreateSeriesAsync(string seriesName)
    {
        const string selectSql = "SELECT id FROM series WHERE name = @Name;";
        const string insertSql = "INSERT INTO series (name) VALUES (@Name) RETURNING id;";

        await using var connection = await _dbContext.GetConnectionAsync();

        var existingId = await connection.QueryFirstOrDefaultAsync<int?>(selectSql, new { Name = seriesName });
        if (existingId.HasValue)
            return existingId.Value;

        return await connection.ExecuteScalarAsync<int>(insertSql, new { Name = seriesName });
    }
}