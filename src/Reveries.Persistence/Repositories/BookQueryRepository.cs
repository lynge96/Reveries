using System.Text.Json;
using Dapper;
using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Domain.Editions;
using Reveries.Domain.Enums;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Views;

namespace Reveries.Persistence.Repositories;

public class BookQueryRepository : IBookQueryRepository
{
    private const string BaseSql = """
                                   SELECT
                                       e.id                   AS "bookId",
                                       e.isbn13,
                                       e.isbn10,
                                       e.language,
                                       e."pageCount",
                                       e."publicationDate",
                                       e.format,
                                       e."editionStatement",
                                       e."coverImageUrl",
                                       e."imageThumbnailUrl",
                                       e."heightCm",
                                       e."widthCm",
                                       e."thicknessCm",
                                       e."weightG",
                                       w.title,
                                       w.subtitle,
                                       w.synopsis,
                                       w.description,
                                       e."publisherName",
                                       w."seriesName",
                                       w."seriesNumber",
                                       w.authors,
                                       w."primaryGenres",
                                       w."secondaryGenres",
                                       w."deweyCodes"
                                   FROM library.editions_view e
                                   JOIN library.works_view w ON w.id = e."workId"
                                   """;

    private readonly IDbContext _dbContext;

    public BookQueryRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<BookDetails?> GetBookByIdAsync(Guid bookId, CancellationToken ct)
    {
        const string sql = $"{BaseSql}\nWHERE e.id = @Id";

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Id = bookId }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<BookDetailsRow>(command);

        return row is null ? null : MapToBookDetails(row);
    }

    public async Task<IReadOnlyList<BookDetails>> GetAllBooksAsync(CancellationToken ct)
    {
        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(BaseSql, null, ct);

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
