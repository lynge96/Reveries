using Reveries.Core.Models;
using Spectre.Console;

namespace Reveries.Console.Common.Extensions;

public static class BookFilterExtensions
{
    public static List<Book> SelectLanguages(this IEnumerable<Book> books)
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
        
        if (selectedLanguages.Count == 0)
            return booksList;
        
        return booksList
            .Where(b => selectedLanguages.Contains(b.Language!))
            .ToList();
    }

}
