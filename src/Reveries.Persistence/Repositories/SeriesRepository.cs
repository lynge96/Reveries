using Dapper;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.BookSeries;
using Reveries.Persistence.Context;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;

namespace Reveries.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IDbContext _dbContext;

    public SeriesRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Series?> GetOrCreateAsync(
        Series? series,
        CancellationToken ct)
    {
        if (series is null)
            return null;

        const string sql = """
                           INSERT INTO library.series (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO UPDATE 
                           SET name = EXCLUDED.name
                           RETURNING id, name, date_created
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var seriesEntity = series.ToEntity();

        var command = _dbContext.CreateCommand(sql, new { seriesEntity.Id, seriesEntity.Name }, ct);

        var result = await connection.QuerySingleAsync<SeriesEntity>(command);

        return result.ToDomain();
    }

    public async Task<Series?> GetByNameAsync(Series series, CancellationToken ct)
    {
        const string sql = """
                           SELECT 
                               id,
                               name, 
                               date_created
                           FROM library.series 
                           WHERE name ILIKE @Name
                           LIMIT 1;
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { series.Name }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<SeriesEntity>(command);

        return row?.ToDomain();
    }

    public async Task<List<Series>> GetSeriesAsync(CancellationToken ct)
    {
        const string sql = """
                           SELECT 
                               id, 
                               name, 
                               date_created
                           FROM library.series;
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, ct: ct);

        var rows = await connection.QueryAsync<SeriesEntity>(command);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}
