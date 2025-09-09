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
        var bookNamesList = booksToPrompt.Select(b => Markup.Escape(b.Title)).ToList();
        if (booksToPrompt.Count == 0)
            return new List<Book>();
        
        var prompt = new MultiSelectionPrompt<string>()
            .Title("Select the books you want to save:".AsPrimary())
            .PageSize(10)
            .InstructionsText("(Press <space> to select, <enter> to confirm)".AsInfo().Italic());
        
        if (bookNamesList.Count != 0)
        {
            prompt.AddChoiceGroup("Cancel");
            prompt.AddChoiceGroup("All books", bookNamesList);
        }

        var selectedBooks = AnsiConsole.Prompt(prompt);
        
        if (selectedBooks.Contains("Cancel"))
            return new List<Book>();
        
        var pickedBooks = selectedBooks
            .Select(title => books.FirstOrDefault(b => b.Title == title))
            .Where(b => b != null)
            .ToList()!;

        return pickedBooks!;
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
