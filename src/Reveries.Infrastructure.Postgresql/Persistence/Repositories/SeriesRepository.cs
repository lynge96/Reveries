using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IDbContext _dbContext;

    public SeriesRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> AddAsync(Series series)
    {
        const string sql = """
                           INSERT INTO library.series (domain_id, name) 
                           VALUES (@SeriesDomainId, @SeriesName)
                           ON CONFLICT DO NOTHING
                           RETURNING id;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var seriesEntity = series.ToDbModel();
        
        var seriesId = await connection.QuerySingleAsync<int>(sql, seriesEntity);

        return seriesId;
    }
    
    public async Task<SeriesWithId?> GetByNameAsync(string seriesName)
    {
        const string sql = """
                           SELECT 
                               id AS SeriesId, 
                               domain_id AS SeriesDomainId, 
                               name AS SeriesName, 
                               date_created AS DateCreatedSeries 
                           FROM library.series 
                           WHERE name ILIKE @Name
                           LIMIT 1;
                           """;
    
        var connection = await _dbContext.GetConnectionAsync();
    
        var seriesDto = await connection.QueryFirstOrDefaultAsync<SeriesEntity>(sql, new { Name = seriesName });
    
        if (seriesDto == null)
            return null;
        
        return new SeriesWithId(seriesDto.ToDomain(), seriesDto.SeriesId);
    }

}