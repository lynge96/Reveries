using System.Data;
using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Repositories;

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
    
        var command = new CommandDefinition(
            commandText: sql,
            parameters: new { seriesEntity.Id, seriesEntity.Name },
            cancellationToken: ct
        );
    
        var result = await connection.QuerySingleAsync<SeriesEntity>(command);
    
        return result.ToDomain();
    }

    public async Task<Series?> GetByNameAsync(string seriesName)
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
    
        var connection = await _dbContext.GetConnectionAsync();
    
        var row = await connection.QueryFirstOrDefaultAsync<SeriesEntity>(sql, new { Name = seriesName });

        return row?.ToDomain();
    }

    public async Task<List<Series>> GetSeriesAsync()
    {
        const string sql = """
                           SELECT 
                               id, 
                               name, 
                               date_created
                           FROM library.series;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var rows = await connection.QueryAsync<SeriesEntity>(sql);
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}