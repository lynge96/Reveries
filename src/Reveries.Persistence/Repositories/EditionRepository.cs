using Reveries.Domain.Editions;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Records;

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
                           INSERT INTO catalog.editions (
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

        await _dbContext.ExecuteAsync(sql, edition.ToRecord(), ct);
    }

    public async Task<Edition?> GetEditionByIsbnAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = """
                           SELECT id, work_id, isbn13, isbn10, publication_date, page_count,
                                  language, edition_statement, format, image_url, image_thumbnail,
                                  saxo_url, height_cm, width_cm, thickness_cm, weight_g, publisher_id
                           FROM catalog.editions
                           WHERE isbn13 = @Value13
                              OR (@Value10 IS NOT NULL AND isbn10 = @Value10)
                           LIMIT 1
                           """;

        var row = await _dbContext.QueryFirstOrDefaultAsync<EditionRecord>(
            sql, new { isbn.Value13, isbn.Value10 }, ct);

        return row?.ToDomain();
    }

    public async Task<bool> EditionExistsAsync(Isbn isbn, CancellationToken ct)
    {
        const string sql = """
                           SELECT EXISTS (
                               SELECT 1 FROM catalog.editions
                               WHERE isbn13 = @Value13
                                  OR (@Value10 IS NOT NULL AND isbn10 = @Value10)
                           )
                           """;

        return await _dbContext.QuerySingleAsync<bool>(sql, new { isbn.Value13, isbn.Value10 }, ct);
    }
}