using Reveries.Console.Common.Extensions;
using Reveries.Core.Entities;
using Spectre.Console;

namespace Reveries.Console.Common.Utilities;

public static class BookSelectionUtility
{
    public static List<Book> SelectBooksToSave(List<Book> books)
    {
        var selectedBooks = new List<Book>();
        
        if (books.Count == 1)
        {
            if (AnsiConsole.Confirm("Do you want to save this book in the database?".AsPrimary()))
            {
                selectedBooks.Add(books.First());
            }
            return selectedBooks;
        }

        var choices = new MultiSelectionPrompt<Book>()
            .Title("Select books you want to save:".AsPrimary())
            .PageSize(10)
            .InstructionsText("(Press <space> to select, <enter> to confirm)".AsInfo().Italic());

        choices.AddChoices(books);

        var selectedBooksList = AnsiConsole.Prompt(choices);

        return selectedBooksList;
    }
}