using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Interfaces.Persistence;

namespace Reveries.Infrastructure.Persistence.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IPostgresDbContext _dbContext;
    
    public PublisherRepository(IPostgresDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> CreatePublisherAsync(Publisher publisher)
    {
        const string sql = """
                           INSERT INTO publishers (name, date_created)
                           VALUES (@Name, @DateCreated)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
    
        var publisherId = await connection.QuerySingleAsync<int>(sql, 
            new { 
                publisher.Name, 
                DateCreated = DateTimeOffset.UtcNow 
            });

        publisher.PublisherId = publisherId;
    
        return publisherId;
    }
    
    public async Task<Publisher?> GetPublisherByNameAsync(string? name)
    {
        const string sql = """
                           SELECT * FROM publishers 
                           WHERE name ILIKE @Name
                           LIMIT 1
                           """;
            
        var connection = await _dbContext.GetConnectionAsync();
        
        return await connection.QuerySingleOrDefaultAsync<Publisher>(sql, new { Name = name });
    }
}