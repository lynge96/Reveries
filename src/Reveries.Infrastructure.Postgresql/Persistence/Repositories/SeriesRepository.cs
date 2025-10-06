using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Core.Interfaces.Persistence.Repositories;
using Reveries.Core.Entities;
using Reveries.Infrastructure.Postgresql.DTOs;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IDbContext _dbContext;

    public SeriesRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Series?> GetSeriesByNameAsync(string? seriesName)
    {
        if (string.IsNullOrWhiteSpace(seriesName))
            return null;
    
        const string sql = """
                           SELECT id as SeriesId, name as seriesName, date_created as DateCreatedSeries 
                           FROM series 
                           WHERE name ILIKE @Name
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

    public async Task<List<Series>> GetSeriesAsync()
    {
        const string sql = """
                           SELECT id as SeriesId, name as seriesName, date_created as DateCreatedSeries
                           FROM series
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var seriesDtos = await connection.QueryAsync<SeriesDto>(sql);
        
        return seriesDtos.Select(dto => dto.ToDomain()).ToList();
    }

}