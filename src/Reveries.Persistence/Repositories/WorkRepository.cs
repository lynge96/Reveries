using System.Text.Json;
using Dapper;
using Reveries.Domain.Authors;
using Reveries.Domain.Interfaces.IRepository;
using Reveries.Domain.Shared;
using Reveries.Domain.Works;
using Reveries.Persistence.Context;
using Reveries.Persistence.Entities;
using Reveries.Persistence.Interfaces;
using Reveries.Persistence.Mappers;
using Reveries.Persistence.Views;

namespace Reveries.Persistence.Repositories;

public class WorkRepository : IWorkRepository
{
    private readonly IDbContext _dbContext;

    public WorkRepository(IDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertWorkAsync(Work work, CancellationToken ct)
    {
        const string sql = """
                           INSERT INTO library.works (
                               id, title, synopsis, series_id, series_number
                           )
                           VALUES (
                               @Id, @Title, @Synopsis, @SeriesId, @SeriesNumber
                           )
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);
        var workEntity = work.ToEntity();

        var command = _dbContext.CreateCommand(sql, workEntity, ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<Work?> GetWorkByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.works_view
                           WHERE id = @Id
                           """;

        var works = await QueryWorksAsync(sql, new { Id = id }, ct);

        return works.FirstOrDefault();
    }

    public async Task UpdateWorkSeriesAsync(Work work, Guid seriesId, CancellationToken ct)
    {
        const string sql = """
                           UPDATE library.works
                           SET series_id = @SeriesId,
                               series_number = @SeriesNumber
                           WHERE id = @Id
                           """;

        var connection = await _dbContext.GetConnectionAsync(ct);

        var command = _dbContext.CreateCommand(
            sql,
            new { Id = work.Id.Value, SeriesId = seriesId, SeriesNumber = work.SeriesPlacement?.Number },
            ct);

        await connection.ExecuteAsync(command);
    }

    public async Task<List<Work>> GetWorksByAuthorsAsync(IEnumerable<Author> authors, CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.works_view
                           WHERE authors ILIKE ANY(@Patterns)
                           """;

        var patterns = authors
            .Where(n => !string.IsNullOrWhiteSpace(n.NormalizedName))
            .Select(n => $"%{n.NormalizedName.Trim()}%")
            .ToList();

        return await QueryWorksAsync(sql, new { Patterns = patterns }, ct);
    }

    public async Task<List<Work>> GetDetailedWorksByTitleAsync(List<Title> titles, CancellationToken ct)
    {
        if (titles.Count == 0)
            return [];

        const string sql = """
                           SELECT *
                           FROM library.works_view
                           WHERE title ILIKE ANY(@Patterns)
                           """;

        var patterns = titles
            .Where(t => !string.IsNullOrWhiteSpace(t.Value))
            .Select(t => $"%{t.Value.Trim()}%")
            .ToList();

        return await QueryWorksAsync(sql, new { Patterns = patterns }, ct);
    }

    public async Task<List<Work>> GetAllWorksAsync(CancellationToken ct)
    {
        const string sql = """
                           SELECT *
                           FROM library.works_view
                           """;

        return await QueryWorksAsync(sql, null, ct);
    }

    private async Task<List<Work>> QueryWorksAsync(string sql, object? parameters, CancellationToken ct)
    {
        var aggregates = await GetWorkAggregatesAsync(sql, parameters, ct);

        return aggregates.Select(WorkMappingExtensions.ToDomainAggregate).ToList();
    }

    private async Task<List<WorkAggregateEntity>> GetWorkAggregatesAsync(string sql, object? parameters, CancellationToken ct)
    {
        var connection = await _dbContext.GetConnectionAsync(ct);
        var command = _dbContext.CreateCommand(sql, parameters, ct);

        var rows = await connection.QueryAsync<WorksView>(command);

        var result = new List<WorkAggregateEntity>();

        foreach (var row in rows)
        {
            var authors = JsonSerializer.Deserialize<List<AuthorEntity>>(row.Authors) ?? [];
            var primaryGenres = JsonSerializer.Deserialize<List<GenreEntity>>(row.PrimaryGenres) ?? [];
            var secondaryGenres = JsonSerializer.Deserialize<List<GenreEntity>>(row.SecondaryGenres) ?? [];
            var deweyDecimals = row.DeweyCodes
                .Select(code => new DeweyDecimalEntity { Code = code })
                .ToList();

            var aggregate = new WorkAggregateEntity
            {
                Work = new WorkEntity
                {
                    Id = row.Id,
                    Title = row.Title,
                    Synopsis = row.Synopsis,
                    SeriesNumber = row.SeriesNumber,
                    SeriesId = row.SeriesId,
                    DateCreated = row.DateCreatedWork
                },

                Series = row.SeriesId is { } seriesId
                    ? new SeriesEntity
                    {
                        Id = seriesId,
                        Name = row.SeriesName!,
                        DateCreated = row.DateCreatedSeries
                    }
                    : null,

                Authors = authors,
                PrimaryGenres = primaryGenres,
                SecondaryGenres = secondaryGenres,
                DeweyDecimals = deweyDecimals
            };

            result.Add(aggregate);
        }

        return result;
    }
}