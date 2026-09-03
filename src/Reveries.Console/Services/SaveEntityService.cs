using Reveries.Application.Books.Interfaces;
using Reveries.Application.Books.Models;
using Reveries.Application.BookSeries.Services;
using Reveries.Application.Common.Exceptions;
using Reveries.Console.Common.Extensions;
using Reveries.Domain.BookSeries;
using Spectre.Console;

namespace Reveries.Console.Services;

public class SaveEntityService
{
    private readonly IWorkPersistenceService _workPersistenceService;
    private readonly CreateSeriesService _createSeriesService;

    public SaveEntityService(
        IWorkPersistenceService workPersistenceService,
        CreateSeriesService createSeriesService)
    {
        _workPersistenceService = workPersistenceService;
        _createSeriesService = createSeriesService;
    }

    public async Task SaveBooksAsync(IEnumerable<EditionWithWork> books, CancellationToken ct = default)
    {
        var booksList = books.ToList();

        if (booksList.Count == 0)
        {
            AnsiConsole.MarkupLine("No books were selected to save.".AsWarning());
            return;
        }

        AnsiConsole.MarkupLine($"\nSaving {booksList.Count} book(s)...".AsSuccess());

        foreach (var book in booksList)
        {
            var edition = book.Edition;
            var work = book.Work;

            try
            {
                var editionId = await _workPersistenceService.SaveWorkWithEditionAsync(work, edition, ct);

                AnsiConsole.MarkupLine($"""
                                        ✅ Successfully saved to database:
                                           Title: {work.Title}
                                           ID: {editionId}
                                           ISBN: {edition.Isbn?.Value13 ?? "N/A"}
                                        """.AsPrimary());
            }
            catch (BookAlreadyExistsException ex)
            {
                AnsiConsole.MarkupLine($"""
                                        ⚠️ Book already exists:
                                           Title: {work.Title}
                                           Error: {ex.Message}
                                        """.AsWarning());
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"""
                                        ❌ Transaction failed:
                                           Title: {work.Title}
                                           Error: {ex.Message}
                                           Details: Transaction was rolled back
                                        """.AsError());
            }
        }
    }

    public async Task SaveSeriesAsync(Series series, CancellationToken ct = default)
    {
        AnsiConsole.MarkupLine($"\nSaving series {series.Name}...".AsSuccess());

        try
        {
            var createdSeries = await _createSeriesService.CreateSeriesAsync(series, ct);

            AnsiConsole.MarkupLine($"""
                                    ✅ Successfully saved to database:
                                       Name: {createdSeries.Name}
                                       ID: {createdSeries.Id.Value}
                                    """.AsPrimary());
        }
        catch (SeriesAlreadyExistsException ex)
        {
            AnsiConsole.MarkupLine($"""
                                    ⚠️ Series already exists:
                                       Name: {series.Name}
                                       Error: {ex.Message}
                                    """.AsWarning());
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"""
                                    ❌ Transaction failed:
                                       Name: {series.Name}
                                       Error: {ex.Message}
                                       Details: Transaction was rolled back
                                    """.AsError());
        }
    }
}
