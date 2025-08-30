using Reveries.Console.Common.Extensions;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Reveries.Core.Enums;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSelectionService : IBookSelectionService
{
    public List<Book> SelectBooksToSave(List<Book> books)
    {
        var booksToPrompt = books.Where(b => b.DataSource != DataSource.Database).ToList();

        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What do you want to do?".AsPrimary())
                .AddChoices("Save all books", "Select specific books", "Skip saving")
        );

        switch (action)
        {
            case "Save all books":
                return booksToPrompt;
            case "Skip saving":
                break;
            case "Select specific books":
                var selectedBooks = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Select the books you want to save:".AsPrimary())
                        .PageSize(10)
                        .InstructionsText("(Press <space> to select, <enter> to confirm)".AsInfo().Italic())
                        .AddChoices(booksToPrompt.Select(b => b.ToString()))
                );
        
                return booksToPrompt
                    .Where(b => selectedBooks.Contains(b.ToString()))
                    .ToList();
        }

        return new List<Book>();
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

        var selectedLanguages = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select languages to filter by:".AsSuccess().Underline())
                .PageSize(10)
                .InstructionsText("Press <space> to select, <enter> to confirm".AsInfo().Italic())
                .AddChoices(availableLanguages));
        
        return selectedLanguages.Count == 0 
            ? booksList 
            : booksList.Where(b => selectedLanguages.Contains(b.Language!)).ToList();
    }
}
