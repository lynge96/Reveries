using Dapper;
using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;
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
                               publication_date, edition_statement, format,
                               image_url, image_thumbnail, saxo_url,
                               height_cm, width_cm, thickness_cm, weight_g,
                               publisher_id
                           )
                           VALUES (
                               @Id, @WorkId, @Isbn13, @Isbn10, @PageCount, @Language,
                               @PublicationDate, @EditionStatement, @Format,
                               @ImageUrl, @ImageThumbnail, @SaxoUrl,
                               @HeightCm, @WidthCm, @ThicknessCm, @WeightG,
                               @PublisherId
                           )
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var editionEntity = edition.ToEntity();

        var command = _dbContext.CreateCommand(sql, editionEntity, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<Edition?> GetEditionByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.editions_view
                           WHERE isbn13 = @Value13
                              OR (@Value10 IS NOT NULL AND isbn10 = @Value10)
                           LIMIT 1
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(
            sql,
            new { isbn.Value13, isbn.Value10 },
            ct);

        var row = await connection.QueryFirstOrDefaultAsync<EditionsView>(command);

        return row?.ToDomain();
    }

    public async Task<bool> EditionExistsAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = """
                           SELECT EXISTS (
                               SELECT 1 FROM library.editions
                               WHERE isbn13 = @Value13
                                  OR (@Value10 IS NOT NULL AND isbn10 = @Value10)
                           )
                           """;
        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(sql, new { isbn.Value13, isbn.Value10 }, ct);

        return await connection.QuerySingleAsync<bool>(command);
    }
}