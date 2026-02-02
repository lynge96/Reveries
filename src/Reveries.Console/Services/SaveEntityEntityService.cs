using Reveries.Application.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Core.Exceptions;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Services;

public class SaveEntityEntityService : ISaveEntityService
{
    private readonly IBookManagementService _bookManagementService;
    private readonly IBookSeriesService _bookSeriesService;

    public SaveEntityEntityService(IBookManagementService bookManagementService, IBookSeriesService bookSeriesService)
    {
        _bookManagementService = bookManagementService;
        _bookSeriesService = bookSeriesService;
    }

    public async Task SaveBooksAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
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
            try
            {
                var bookId = await _bookManagementService.CreateBookWithRelationsAsync(book, cancellationToken);

                AnsiConsole.MarkupLine($"""
                                        ✅ Successfully saved to database:
                                           Title: {book.Title}
                                           ID: {bookId}
                                           ISBN: {book.Isbn13?.Value ?? book.Isbn10?.Value ?? "N/A"}
                                        """.AsPrimary());
            }
            catch (BookAlreadyExistsException ex)
            {
                AnsiConsole.MarkupLine($"""
                                        ⚠️ Book already exists:
                                           Title: {book.Title}
                                           Error: {ex.Message}
                                        """.AsWarning());
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"""
                                        ❌ Transaction failed:
                                           Title: {book.Title}
                                           Error: {ex.Message}
                                           Details: Transaction was rolled back
                                        """.AsError());
            }
        }
    }

    public async Task SaveSeriesAsync(Series series, CancellationToken cancellationToken = default)
    {
        AnsiConsole.MarkupLine($"\nSaving series {series.Name}...".AsSuccess());

        try
        {
            var seriesId = await _bookSeriesService.CreateSeriesAsync(series);
            
            AnsiConsole.MarkupLine($"""
                                    ✅ Successfully saved to database:
                                       Name: {series.Name}
                                       ID: {seriesId}
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
