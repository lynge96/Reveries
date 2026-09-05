using System.Text.Json;
using Dapper;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Rows;

namespace Reveries.Persistence.Repositories;

public class BookQueryRepository : IBookQueryRepository
{
    private const string BaseSql = """
                                   SELECT
                                       e.id AS book_id,
                                       e.isbn13,
                                       e.isbn10,
                                       e.language,
                                       e.page_count,
                                       e.publication_date,
                                       e.format,
                                       e.edition_statement,
                                       e.image_url AS cover_image_url,
                                       e.image_thumbnail AS image_thumbnail_url,
                                       e.height_cm,
                                       e.width_cm,
                                       e.thickness_cm,
                                       e.weight_g,
                                       w.title,
                                       w.subtitle,
                                       w.synopsis,
                                       w.description,
                                       p.name AS publisher_name,
                                       se.name AS series_name,
                                       w.series_number,
                                       COALESCE(a.authors, '[]'::jsonb) AS authors,
                                       COALESCE(g.primary_genres, '[]'::jsonb) AS primary_genres,
                                       COALESCE(g.secondary_genres, '[]'::jsonb) AS secondary_genres,
                                       COALESCE(dd.dewey_codes, ARRAY[]::text[]) AS dewey_codes
                                   FROM catalog.editions e
                                   JOIN catalog.works w ON w.id = e.work_id
                                   LEFT JOIN catalog.publishers p ON p.id = e.publisher_id
                                   LEFT JOIN catalog.series se ON se.id = w.series_id
                                   LEFT JOIN LATERAL (
                                       SELECT
                                           jsonb_agg(jsonb_build_object('Id', gg.id, 'Name', gg.name) ORDER BY gg.name)
                                               FILTER (WHERE wg.is_primary) AS primary_genres,
                                           jsonb_agg(jsonb_build_object('Id', gg.id, 'Name', gg.name) ORDER BY gg.name)
                                               FILTER (WHERE NOT wg.is_primary) AS secondary_genres
                                       FROM catalog.works_genres wg
                                       JOIN catalog.genres gg ON gg.id = wg.genre_id
                                       WHERE wg.work_id = w.id
                                   ) g ON true
                                   LEFT JOIN LATERAL (
                                       SELECT jsonb_agg(jsonb_build_object('Id', aa.id, 'Name', aa.name) ORDER BY aa.name) AS authors
                                       FROM catalog.works_authors wa
                                       JOIN catalog.authors aa ON aa.id = wa.author_id
                                       WHERE wa.work_id = w.id
                                   ) a ON true
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

    public async Task<BookDetails?> GetBookByIdAsync(Guid bookId, CancellationToken ct)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(BaseSql);
        builder.Where("e.id = @Id", new { Id = bookId });

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(template.RawSql, template.Parameters, ct);

        var row = await connection.QueryFirstOrDefaultAsync<BookDetailsRow>(command);

        return row is null ? null : MapToBookDetails(row);
    }

    public async Task<IReadOnlyList<BookDetails>> GetAllBooksAsync(CancellationToken ct)
    {
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(BaseSql);

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(template.RawSql, template.Parameters, ct);

        var rows = await connection.QueryAsync<BookDetailsRow>(command);

        return rows.Select(MapToBookDetails).ToList();
    }

    private static BookDetails MapToBookDetails(BookDetailsRow row)
    {
        return new BookDetails
        {
            BookId = row.BookId,
            Isbn10 = row.Isbn10,
            Isbn13 = row.Isbn13,
            Title = row.Title,
            Subtitle = row.Subtitle,
            Series = row.SeriesName,
            NumberInSeries = row.SeriesNumber,
            Authors = DeserializeNames(row.Authors),
            Publisher = row.PublisherName,
            Language = Language.TryCreate(row.Language)?.DisplayName,
            Pages = row.PageCount,
            PublicationDate = row.PublicationDate,
            Synopsis = row.Synopsis,
            Description = row.Description,
            Format = NormalizeFormat(row.Format),
            Edition = row.EditionStatement,
            CoverImageUrl = row.CoverImageUrl,
            ImageThumbnailUrl = row.ImageThumbnailUrl,
            HeightCm = row.HeightCm,
            WidthCm = row.WidthCm,
            ThicknessCm = row.ThicknessCm,
            WeightG = row.WeightG,
            DeweyDecimals = row.DeweyCodes,
            PrimaryGenres = DeserializeNames(row.PrimaryGenres),
            SecondaryGenres = DeserializeNames(row.SecondaryGenres)
        };
    }

    private static string NormalizeFormat(string? value)
    {
        return Enum.TryParse<BookFormat>(value, out var format)
            ? format.ToString()
            : BookFormat.Unknown.ToString();
    }

    private static IReadOnlyList<string> DeserializeNames(string json)
    {
        var items = JsonSerializer.Deserialize<List<NameProjection>>(json) ?? [];

        return items
            .Select(item => item.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();
    }

    private sealed record NameProjection(string Name);
}