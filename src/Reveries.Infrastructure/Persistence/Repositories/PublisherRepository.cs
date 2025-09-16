using Dapper;
using Reveries.Core.Entities;
using Reveries.Core.Interfaces.Repositories;
using Reveries.Infrastructure.Persistence.DTOs;
using Reveries.Infrastructure.Persistence.Interfaces;
using Reveries.Infrastructure.Persistence.Mappers;

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
                           INSERT INTO publishers (name)
                           VALUES (@Name)
                           RETURNING id
                           """;
        
        var connection = await _dbContext.GetConnectionAsync();
        
        var publisherDto = publisher.ToDto();
        
        var publisherId = await connection.QuerySingleAsync<int>(sql, publisherDto);

        publisher.Id = publisherId;
        return publisherId;
    }
    
    public async Task<List<Publisher>> GetPublishersByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new List<Publisher>();

        const string sql = """
                           SELECT id AS publisherId, name, date_created AS dateCreatedPublisher
                           FROM publishers
                           WHERE name ILIKE @Name
                           """;

        var connection = await _dbContext.GetConnectionAsync();

        var publisherDtos = await connection.QueryAsync<PublisherDto>(sql, new { Name = name });

        return publisherDtos.Select(dto => dto.ToDomain()).ToList();
    }
}