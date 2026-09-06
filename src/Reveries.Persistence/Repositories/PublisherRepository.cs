using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Records;

namespace Reveries.Persistence.Repositories;

public class PublisherRepository : IPublisherRepository
{
    private readonly IDbContext _dbContext;

    public PublisherRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Publisher?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.publishers
                           WHERE name = @Name::citext
                           LIMIT 1
                           """;

        var row = await _dbContext.QueryFirstOrDefaultAsync<PublisherRecord>(sql, new { Name = name }, ct);

        return row?.ToDomain();
    }

    public async Task AddAsync(Publisher publisher, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO catalog.publishers (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO NOTHING
                           """;

        await _dbContext.ExecuteAsync(sql, publisher.ToRecord(), ct);
    }

    public async Task<List<Publisher>> SearchByNameAsync(Publisher publisher, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.publishers
                           WHERE name ILIKE @Name
                           ORDER BY name
                           """;

        var rows = await _dbContext.QueryAsync<PublisherRecord>(sql, new { Name = $"%{publisher.Name}%" }, ct);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}