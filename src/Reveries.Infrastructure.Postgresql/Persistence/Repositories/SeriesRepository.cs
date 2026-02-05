using Dapper;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly IDbContext _dbContext;

    public SeriesRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> AddAsync(SeriesEntity series)
    {
        const string sql = """
                           INSERT INTO library.series (domain_id, name) 
                           VALUES (@SeriesDomainId, @SeriesName)
                           ON CONFLICT DO NOTHING
                           RETURNING id;
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var seriesId = await connection.QuerySingleAsync<int>(sql, series);

        return seriesId;
    }
    
    public async Task<SeriesEntity?> GetByNameAsync(string seriesName)
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
    
        return seriesDto;
    }

}