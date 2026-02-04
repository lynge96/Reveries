using Dapper;
using Reveries.Application.Interfaces.Persistence;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IDbContext _dbContext;
    
    public PublisherRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> AddAsync(PublisherEntity publisher)
    {
        const string sql = """
                           INSERT INTO library.publishers (domain_id, name)
                           VALUES (@PublisherDomainId, @Name)
                           ON CONFLICT DO NOTHING
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var publisherDbId = await connection.QuerySingleAsync<int>(sql, publisher);

        return publisherDbId;
    }
    
    public async Task<PublisherEntity?> GetByNameAsync(string publisherName)
    {
        const string sql = """
                           SELECT 
                               id AS publisherId, 
                               domain_id AS publisherDomainId,
                               name AS publisherName, 
                               date_created AS dateCreatedPublisher
                           FROM library.publishers
                           WHERE name ILIKE @Name
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var publisherDtos = await connection.QueryFirstOrDefaultAsync<PublisherEntity>(sql, new { Name = publisherName });

        return publisherDtos;
    }
}