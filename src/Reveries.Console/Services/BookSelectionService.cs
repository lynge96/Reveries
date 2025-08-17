using Reveries.Console.Common.Extensions;
using Reveries.Console.Services.Interfaces;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Services;

public class BookSelectionService : IBookSelectionService
{
    public List<Book> SelectBooksToSave(List<Book> books)
    {
        if (books.Count == 1)
        {
            return AnsiConsole.Confirm("Do you want to save this book in the database?".AsPrimary())
                ? new List<Book> { books.First() }
                : new List<Book>();
        }

        var choices = new MultiSelectionPrompt<Book>()
            .Title("Select books you want to save:".AsPrimary())
            .PageSize(10)
            .InstructionsText("(Press <space> to select, <enter> to confirm)".AsInfo().Italic());

        choices.AddChoices(books);
        return AnsiConsole.Prompt(choices);
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
