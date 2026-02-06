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
                           VALUES (@PublisherDomainId, @Name)
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

        var publisherDto = await connection.QueryFirstOrDefaultAsync<PublisherEntity>(sql, new { Name = publisherName });

        if (publisherDto == null)
            return null;

        return new PublisherWithId(publisherDto.ToDomain(), publisherDto.PublisherId);
    }
}