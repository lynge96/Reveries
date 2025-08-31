using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;
using Reveries.Infrastructure.Persistence.DTOs;
using Reveries.Infrastructure.Persistence.Mappers;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IPostgresDbContext _dbContext;

    public SeriesRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Series?> GetSeriesByNameAsync(string? seriesName)
    {
        if (string.IsNullOrWhiteSpace(seriesName))
            return null;
    
        const string sql = """
                           SELECT id as SeriesId, name, date_created as DateCreatedSeries 
                           FROM series 
                           WHERE name = @Name
                           LIMIT 1;
                           """;
    
        var connection = await _dbContext.GetConnectionAsync();
    
        var seriesDto = await connection.QueryFirstOrDefaultAsync<SeriesDto>(sql, new { Name = seriesName });
    
        return seriesDto?.ToDomain();
    }

    public async Task<int> CreateSeriesAsync(Series series)
    {
        const string sql = """
                           INSERT INTO series (name) 
                           VALUES (@SeriesName) 
                           RETURNING id;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var seriesDto = series.ToDto();
        
        var seriesId = await connection.QuerySingleAsync<int>(sql, seriesDto);
        
        series.Id = seriesId;
        return seriesId;
    }
}