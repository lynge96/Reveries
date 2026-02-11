using Dapper;
using Reveries.Core.Interfaces.IRepository;
using Reveries.Core.Models;
using Reveries.Core.ValueObjects.DTOs;
using Reveries.Infrastructure.Postgresql.Entities;
using Reveries.Infrastructure.Postgresql.Interfaces;
using Reveries.Infrastructure.Postgresql.Mappers;

namespace Reveries.Infrastructure.Postgresql.Persistence.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IDbContext _dbContext;
    
    public PublisherRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<int> AddAsync(Publisher publisher)
    {
        const string sql = """
                           INSERT INTO library.publishers (domain_id, name)
                           VALUES (@PublisherDomainId, @PublisherName)
                           ON CONFLICT DO NOTHING
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();

        var publisherEntity = publisher.ToDbModel();
        
        var publisherDbId = await connection.QuerySingleAsync<int>(sql, publisherEntity);

        return publisherDbId;
    }
    
    public async Task<PublisherWithId?> GetByNameAsync(string publisherName)
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

        var row = await connection.QueryFirstOrDefaultAsync<PublisherEntity>(sql, new { Name = publisherName });

        if (row == null)
            return null;

        return new PublisherWithId(row.ToDomain(), row.PublisherId);
    }

    public async Task<List<Publisher>> GetPublishersByNameAsync(string name)
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
        
        var rows = await connection.QueryAsync<PublisherEntity>(sql, new { Name = name });
        
        return rows.Select(r => r.ToDomain()).ToList();
    }
}