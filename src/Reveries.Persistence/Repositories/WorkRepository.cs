using System.Text.Json;
using Dapper;
using Reveries.Domain.Authors;
using Reveries.Domain.BookSeries;
using Reveries.Domain.Interfaces.Repositories;
using Reveries.Domain.Works;
using Reveries.Persistence.Context;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Records;
using Reveries.Persistence.Rows;

namespace Reveries.Persistence.Repositories;

public class WorkRepository : IWorkRepository
{
    private const string WorkAggregateSql = """
                                            SELECT
                                                w.id,
                                                w.title,
                                                w.subtitle,
                                                w.synopsis,
                                                w.description,
                                                w.series_number,
                                                w.series_id,
                                                se.name AS series_name,
                                                COALESCE(g.primary_genres, '[]'::jsonb) AS primary_genres,
                                                COALESCE(g.secondary_genres, '[]'::jsonb) AS secondary_genres,
                                                COALESCE(a.authors, '[]'::jsonb) AS authors,
                                                COALESCE(dd.dewey_codes, ARRAY[]::text[]) AS dewey_codes
                                            FROM catalog.works w
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
                                            """;

    private readonly IDbContext _dbContext;

    public WorkRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertWorkAsync(Work work, WorkRelations relations, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO catalog.works (
                               id, title, subtitle, synopsis, description, series_id, series_number
                           )
                           VALUES (
                               @Id, @Title, @Subtitle, @Synopsis, @Description, @SeriesId, @SeriesNumber
                           )
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        await connection.ExecuteAsync(_dbContext.CreateCommand(sql, work.ToRecord(), ct));

        await InsertAuthorsAsync(work.Id, work.AuthorIds, ct);
        await InsertGenresAsync(work.Id, relations.PrimaryGenreIds, isPrimary: true, ct);
        await InsertGenresAsync(work.Id, relations.SecondaryGenreIds, isPrimary: false, ct);
        await InsertDeweyDecimalsAsync(work.Id, relations.DeweyDecimalIds, ct);
    }

    private async Task InsertAuthorsAsync(WorkId workId, IEnumerable<AuthorId> authorIds, CancellationToken ct)
    {
        var ids = authorIds.Select(a => a.Value).Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO catalog.works_authors (work_id, author_id)
                           SELECT @WorkId, author_id
                           FROM unnest(@AuthorIds::uuid[]) AS author_id
                           ON CONFLICT (work_id, author_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { WorkId = workId.Value, AuthorIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }

    private async Task InsertGenresAsync(WorkId workId, IEnumerable<int> genreIds, bool isPrimary, CancellationToken ct)
    {
        var ids = genreIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO catalog.works_genres (work_id, genre_id, is_primary)
                           SELECT @WorkId, genre_id, @IsPrimary
                           FROM unnest(@GenreIds::int[]) AS genre_id
                           ON CONFLICT (work_id, genre_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { WorkId = workId.Value, GenreIds = ids, IsPrimary = isPrimary }, ct);

        await connection.ExecuteAsync(command);
    }

    private async Task InsertDeweyDecimalsAsync(WorkId workId, IEnumerable<int> deweyDecimalIds, CancellationToken ct)
    {
        var ids = deweyDecimalIds.Distinct().ToArray();
        if (ids.Length == 0)
            return;

        const string sql = """
                           INSERT INTO catalog.works_dewey_decimals (work_id, dewey_decimal_id)
                           SELECT @WorkId, dewey_decimal_id
                           FROM unnest(@DeweyDecimalIds::int[]) AS dewey_decimal_id
                           ON CONFLICT (work_id, dewey_decimal_id) DO NOTHING
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { WorkId = workId.Value, DeweyDecimalIds = ids }, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<Work?> GetWorkByIdAsync(WorkId id, CancellationToken ct)
    {
        const string sql = $"{WorkAggregateSql}\nWHERE w.id = @Id";

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, new { Id = id.Value }, ct);

        var row = await connection.QueryFirstOrDefaultAsync<WorkAggregateRow>(command);

        return row is null ? null : MapToAggregate(row).ToDomainAggregate();
    }

    public async Task UpdateWorkSeriesAsync(Work work, SeriesId seriesId, CancellationToken ct)
    {
        const string sql = """
                           UPDATE catalog.works
                           SET series_id = @SeriesId,
                               series_number = @SeriesNumber,
                               updated_at = now()
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(
            sql,
            new { Id = work.Id.Value, SeriesId = seriesId.Value, SeriesNumber = work.NumberInSeries },
            ct);

        await connection.ExecuteAsync(command);
    }

    private static WorkAggregateRecord MapToAggregate(WorkAggregateRow row)
    {
        var authors = JsonSerializer.Deserialize<List<AuthorRecord>>(row.Authors) ?? [];
        var primaryGenres = JsonSerializer.Deserialize<List<GenreRecord>>(row.PrimaryGenres) ?? [];
        var secondaryGenres = JsonSerializer.Deserialize<List<GenreRecord>>(row.SecondaryGenres) ?? [];
        var deweyDecimals = row.DeweyCodes
            .Select(code => new DeweyDecimalRecord { Code = code })
            .ToList();

        return new WorkAggregateRecord
        {
            Work = new WorkRecord
            {
                Id = row.Id,
                Title = row.Title,
                Subtitle = row.Subtitle,
                Synopsis = row.Synopsis,
                Description = row.Description,
                SeriesNumber = row.SeriesNumber,
                SeriesId = row.SeriesId
            },
            Series = row.SeriesId is { } seriesId
                ? new SeriesRecord { Id = seriesId, Name = row.SeriesName! }
                : null,
            Authors = authors,
            PrimaryGenres = primaryGenres,
            SecondaryGenres = secondaryGenres,
            DeweyDecimals = deweyDecimals
        };
    }
}