using Dapper;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Publishers;
using Reveries.Persistence.Context;
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

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Name = name }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<PublisherRecord>(command);

        return row?.ToDomain();
    }

    public async Task AddAsync(Publisher publisher, CancellationToken ct = default)
    {
        const string sql = """
                           INSERT INTO catalog.publishers (id, name)
                           VALUES (@Id, @Name)
                           ON CONFLICT (name) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, publisher.ToRecord(), ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<List<Publisher>> SearchByNameAsync(Publisher publisher, CancellationToken ct = default)
    {
        const string sql = """
                           SELECT id, name
                           FROM catalog.publishers
                           WHERE name ILIKE @Name
                           ORDER BY name
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Name = $"%{publisher.Name}%" }, ct);

        var rows = await connection.QueryAsync<PublisherRecord>(command);

        return rows.Select(r => r.ToDomain()).ToList();
    }
}