using Dapper;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.BookSeries;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IDbContext _dbContext;

    public SeriesRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Series?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.series
                           WHERE name = @Name::citext
                           LIMIT 1
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Name = name }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<SeriesRecord>(command);

        return row?.ToDomain();
    }

    public async Task AddAsync(Series series, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO catalog.series (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, series.ToRecord(), ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<List<Series>> GetSeriesAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.series
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, ct: ct);

        var rows = await connection.QueryAsync<SeriesRecord>(command);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}