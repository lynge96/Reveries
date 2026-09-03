using Reveries.Application.Books.Models;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Utilities;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSelectionService
{
    public List<BookCandidate> SelectBooksToSave(List<BookCandidate> books)
    {
        if (books.Count == 0)
            return [];

        var sortedBooks = books
            .OrderBy(b => b.Title)
            .ToList();

        var selectedBooks = ConsolePromptUtility.ShowMultiSelectionPrompt("Select books to save:", sortedBooks);

        if (selectedBooks.Count == 0)
        {
            AnsiConsole.MarkupLine("No books selected.".AsWarning());
            return [];
        }

        return selectedBooks;
    }

    public List<T> FilterBooksByLanguage<T>(IEnumerable<T> books) where T : IBookRow
    {
        var booksList = books.ToList();
        var availableLanguages = booksList
            .Where(b => b.LanguageLabel is not null)
            .Select(b => b.LanguageLabel!)
            .Distinct()
            .OrderBy(l => l)
            .ToList();

        if (availableLanguages.Count <= 1)
            return booksList;

        var selectedLanguages = ConsolePromptUtility.ShowMultiSelectionPrompt("Select languages to filter by:", availableLanguages);

        return selectedLanguages.Count == 0
            ? booksList
            : booksList.Where(b => b.LanguageLabel is not null && selectedLanguages.Contains(b.LanguageLabel)).ToList();
    }
}