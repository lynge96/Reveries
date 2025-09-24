using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSelectionService : IBookSelectionService
{
    public List<Book> SelectBooksToSave(List<Book> books)
    {
        var booksToPrompt = books.Where(b => b.DataSource != DataSource.Database && b.DataSource != DataSource.Cache).ToList();
        if (booksToPrompt.Count == 0)
            return new List<Book>();
        
        var sortedBooks = booksToPrompt
            .OrderByDescending(b => b.DataSource.HasFlag(DataSource.Database))
            .ThenBy(b => b.Title)
            .ThenBy(b => b.DataSource.HasFlag(DataSource.CombinedBookApi))
            .ToList();
        
        var selectedBooks = ConsolePromptUtility.ShowMultiSelectionPrompt("Select books to save:", sortedBooks);

        if (selectedBooks.Count == 0)
        {
            AnsiConsole.MarkupLine("No books selected.".AsWarning());
            return new List<Book>();
        }
        
        return selectedBooks;
    }

    public List<Book> FilterBooksByLanguage(IEnumerable<Book> books)
    {
        var booksList = books.ToList();
        var availableLanguages = booksList
            .Where(b => !string.IsNullOrWhiteSpace(b.Language))
            .Select(b => b.Language!)
            .Distinct()
            .OrderBy(l => l)
            .ToList();

        if (availableLanguages.Count <= 1)
            return booksList;

        var selectedLanguages = ConsolePromptUtility.ShowMultiSelectionPrompt("Select languages to filter by:", availableLanguages);
        
        return selectedLanguages.Count == 0 
            ? booksList 
            : booksList.Where(b => selectedLanguages.Contains(b.Language!)).ToList();
    }
}
