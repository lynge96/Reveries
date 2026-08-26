using Reveries.Application.Books.Models;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Utilities;
using Reveries.Domain.Enums;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSelectionService
{
    public List<EditionWithWork> SelectBooksToSave(List<EditionWithWork> books)
    {
        var booksToPrompt = books
            .Where(b => b.Edition.DataSource != DataSource.Database && b.Edition.DataSource != DataSource.Cache)
            .ToList();
        if (booksToPrompt.Count == 0)
            return [];

        var sortedBooks = booksToPrompt
            .OrderByDescending(b => b.Edition.DataSource.HasFlag(DataSource.Database))
            .ThenBy(b => b.Work.Title.Text)
            .ThenBy(b => b.Edition.DataSource.HasFlag(DataSource.CombinedBookApi))
            .ToList();

        var selectedBooks = ConsolePromptUtility.ShowMultiSelectionPrompt("Select books to save:", sortedBooks);

        if (selectedBooks.Count == 0)
        {
            AnsiConsole.MarkupLine("No books selected.".AsWarning());
            return [];
        }

        return selectedBooks;
    }

    public List<EditionWithWork> FilterBooksByLanguage(IEnumerable<EditionWithWork> books)
    {
        var booksList = books.ToList();
        var availableLanguages = booksList
            .Where(b => b.Edition.Language is not null)
            .Select(b => b.Edition.Language!.DisplayName)
            .Distinct()
            .OrderBy(l => l)
            .ToList();

        if (availableLanguages.Count <= 1)
            return booksList;

        var selectedLanguages = ConsolePromptUtility.ShowMultiSelectionPrompt("Select languages to filter by:", availableLanguages);

        return selectedLanguages.Count == 0
            ? booksList
            : booksList.Where(b => b.Edition.Language is not null && selectedLanguages.Contains(b.Edition.Language.DisplayName)).ToList();
    }
}
