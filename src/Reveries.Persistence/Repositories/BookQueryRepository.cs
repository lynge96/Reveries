using Dapper;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Rows;

namespace Reveries.Persistence.Repositories;

public class BookQueryRepository : IBookQueryRepository
{
    private const string BaseSql = """
                                   SELECT
                                       e.id,
                                       e.isbn13,
                                       e.isbn10,
                                       e.language,
                                       e.page_count,
                                       e.publication_date,
                                       e.format,
                                       e.edition_statement,
                                       e.image_url,
                                       e.image_thumbnail,
                                       e.saxo_url,
                                       e.height_cm,
                                       e.width_cm,
                                       e.thickness_cm,
                                       e.weight_g,
                                       w.title,
                                       w.subtitle,
                                       w.synopsis,
                                       w.description,
                                       p.name AS publisher,
                                       COALESCE(a.authors, ARRAY[]::text[]) AS authors,
                                       COALESCE(g.primary_genres, ARRAY[]::text[]) AS primary_genres,
                                       COALESCE(g.secondary_genres, ARRAY[]::text[]) AS secondary_genres,
                                       COALESCE(dd.dewey_codes, ARRAY[]::text[]) AS dewey_codes
                                   FROM catalog.editions e
                                   JOIN catalog.works w ON w.id = e.work_id
                                   LEFT JOIN catalog.publishers p ON p.id = e.publisher_id
                                   -- the work's genres, split into primary/secondary name arrays
                                   LEFT JOIN LATERAL (
                                       SELECT
                                           array_agg(gg.name::text ORDER BY gg.name) FILTER (WHERE wg.is_primary) AS primary_genres,
                                           array_agg(gg.name::text ORDER BY gg.name) FILTER (WHERE NOT wg.is_primary) AS secondary_genres
                                       FROM catalog.works_genres wg
                                       JOIN catalog.genres gg ON gg.id = wg.genre_id
                                       WHERE wg.work_id = w.id
                                   ) g ON true
                                   -- the work's authors as a name array
                                   LEFT JOIN LATERAL (
                                       SELECT array_agg(aa.name::text ORDER BY aa.name) AS authors
                                       FROM catalog.works_authors wa
                                       JOIN catalog.authors aa ON aa.id = wa.author_id
                                       WHERE wa.work_id = w.id
                                   ) a ON true
                                   -- the work's Dewey codes as a text array
                                   LEFT JOIN LATERAL (
                                       SELECT array_agg(DISTINCT ddd.code ORDER BY ddd.code) AS dewey_codes
                                       FROM catalog.works_dewey_decimals wdd
                                       JOIN catalog.dewey_decimals ddd ON ddd.id = wdd.dewey_decimal_id
                                       WHERE wdd.work_id = w.id
                                   ) dd ON true
                                   /**where**/
                                   """;

    private readonly IDbContext _dbContext;

    public BookQueryRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Book?> GetBookByIdAsync(Guid bookId, CancellationToken ct)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(BaseSql);
        builder.Where("e.id = @Id", new { Id = bookId });

        var row = await _dbContext.QueryFirstOrDefaultAsync<BookRow>(
            template.RawSql, template.Parameters, ct);

        return row?.ToBook();
    }

    public async Task<IReadOnlyList<Book>> GetAllBooksAsync(CancellationToken ct)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(BaseSql);

        var rows = await _dbContext.QueryAsync<BookRow>(template.RawSql, template.Parameters, ct);

        return rows.Select(row => row.ToBook()).ToList();
    }
}