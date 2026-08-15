using Dapper;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Shared;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Views;

namespace Reveries.Persistence.Repositories;

public class EditionRepository : IEditionRepository
{
    private readonly IDbContext _dbContext;

    public EditionRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertEditionAsync(Edition edition, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO library.editions (
                               id, work_id, isbn13, isbn10, page_count, language,
                               publication_date, edition_statement, binding,
                               image_url, image_thumbnail, msrp,
                               height_cm, width_cm, thickness_cm, weight_g,
                               data_source, publisher_id
                           )
                           VALUES (
                               @Id, @WorkId, @Isbn13, @Isbn10, @PageCount, @Language,
                               @PublicationDate, @EditionStatement, @Binding,
                               @ImageUrl, @ImageThumbnail, @Msrp,
                               @HeightCm, @WidthCm, @ThicknessCm, @WeightG,
                               @DataSource, @PublisherId
                           )
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var editionEntity = edition.ToEntity();

        var command = _dbContext.CreateCommand(sql, editionEntity, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<Edition?> GetEditionByIsbnAsync(Isbn? isbn13, Isbn? isbn10, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           WHERE isbn13 = @Isbn13
                              OR isbn10 = @Isbn13
                              OR (@Isbn10 IS NOT NULL AND (isbn13 = @Isbn10 OR isbn10 = @Isbn10))
                           LIMIT 1
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(
            sql,
            new { Isbn13 = isbn13?.Value, Isbn10 = isbn10?.Value },
            ct);

        var row = await connection.QueryFirstOrDefaultAsync<EditionsView>(command);

        return row?.ToDomain();
    }

    public async Task<bool> EditionExistsAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM library.editions WHERE isbn13 = @Isbn OR isbn10 = @Isbn)";
        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { Isbn = isbn.Value }, ct);

        return await connection.QuerySingleAsync<bool>(command);
    }

    public async Task<Edition?> GetEditionByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Id = id }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<EditionsView>(command);

        return row?.ToDomain();
    }

    public async Task<List<Edition>> GetEditionsByWorkIdAsync(Guid workId, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           WHERE "workId" = @WorkId
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { WorkId = workId }, ct);

        var rows = await connection.QueryAsync<EditionsView>(command);

        return rows.Select(row => row.ToDomain()).ToList();
    }

    public async Task<List<Edition>> GetAllEditionsAsync(CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, null, ct);

        var rows = await connection.QueryAsync<EditionsView>(command);

        return rows.Select(row => row.ToDomain()).ToList();
    }

    public async Task<List<Edition>> GetEditionsByIsbnsAsync(IEnumerable<Isbn> isbns, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           WHERE isbn13 = ANY(@Isbns)
                              OR isbn10 = ANY(@Isbns)
                           """;

        var isbnList = isbns.Select(i => i.Value).ToList();

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Isbns = isbnList }, ct);

        var rows = await connection.QueryAsync<EditionsView>(command);

        return rows.Select(row => row.ToDomain()).ToList();
    }
}