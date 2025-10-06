using Reveries.Application.Common.Exceptions;
using Reveries.Application.Interfaces.Services;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSaveService : IBookSaveService
{
    private readonly IBookManagementService _bookManagementService;

    public BookSaveService(IBookManagementService bookManagementService)
    {
        _bookManagementService = bookManagementService;
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
                                           ISBN: {book.Isbn13 ?? book.Isbn10 ?? "N/A"}
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
}
