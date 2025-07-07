using Reveries.Application.Interfaces.Services;
using Reveries.Console.Interfaces;
using Reveries.Console.Models.Menu;
using Spectre.Console;

namespace Reveries.Console.Services;

public class ConsoleAppRunner : IConsoleAppRunner
{
    private readonly IBookService _bookService;
    
    public ConsoleAppRunner(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    public async Task RunAsync()
    {
        while (true)
        {
            // 9780804139021
            var selectedOption = AnsiConsole.Prompt(
                new SelectionPrompt<MenuOption>()
                    .Title("[springgreen1]What would you like to search for? 🔎[/]")
                    .AddChoices(MenuConfiguration.Options));
            
            switch (selectedOption.Choice)
            {
                case MenuChoice.SearchBook:
                    await SearchBookByIsbnAsync();
                    break;
                case MenuChoice.SearchAuthor:
                    break;
                case MenuChoice.SearchPublisher:
                    break;
                case MenuChoice.Exit:
                    AnsiConsole.MarkupLine("[springgreen1]Goodbye![/]");
                    return;
            }
            
            if (selectedOption.Choice != MenuChoice.Exit)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                AnsiConsole.Console.Input.ReadKey(true);
                AnsiConsole.Clear();
            }
        }
    }

    private async Task SearchBookByIsbnAsync()
    {
        throw new NotImplementedException();
    }
}