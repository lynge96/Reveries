using Reveries.Application.Interfaces.Services;
using Reveries.Console.Interfaces;
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
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[springgreen1]What would you like to do?[/]")
                    .AddChoices([
                        "📖 Search book by ISBN",
                        "🚪 Exit"
                    ]));
            
            /*switch (choice)
            {
                case "📖 Search book by ISBN":
                    await SearchBookByIsbnAsync();
                    break;
                case "👤 Search author by name":
                    await SearchAuthorAsync();
                    break;
                case "🏢 Search publisher":
                    await SearchPublisherAsync();
                    break;
                case "🚪 Exit":
                    return;
            }*/
        }
    }
}