using Reveries.Domain.BookSeries;
using Reveries.Domain.Interfaces.Repositories;
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

        var row = await _dbContext.QueryFirstOrDefaultAsync<SeriesRecord>(sql, new { Name = name }, ct);

        return row?.ToDomain();
    }

    public async Task AddAsync(Series series, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO catalog.series (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO NOTHING
                           """;

        await _dbContext.ExecuteAsync(sql, series.ToRecord(), ct);
    }

    public async Task<List<Series>> GetSeriesAsync(CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.series
                           """;

        var rows = await _dbContext.QueryAsync<SeriesRecord>(sql, ct: ct);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}