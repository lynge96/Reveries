using Reveries.Console.Extensions;
using Reveries.Console.Interfaces;
using Reveries.Console.Models;
using Reveries.Console.Models.Menu;
using Spectre.Console;

namespace Reveries.Console.Services;

public class ConsoleAppRunner : IConsoleAppRunner
{
    private readonly IMenuOperationService _menuOperationService;
    private bool _isRunning = true;
    
    public ConsoleAppRunner(IMenuOperationService menuOperationService)
    {
        _menuOperationService = menuOperationService;
    }
    
    public async Task RunAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("Welcome to Reveries! 💫".Bold().AsHeader());
        
        while (_isRunning)
        {
            var option = ShowMenu();
        
            if (option.Choice == MenuChoice.Exit)
            {
                _isRunning = false;
                AnsiConsole.MarkupLine("\nGoodbye! ✨".Bold().AsPrimary());
                break;
            }
            
            try
            {
                await _menuOperationService.HandleMenuChoiceAsync(option.Choice);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"{"Error:".Underline().AsError()} {ex.Message.Italic().AsWarning()}");
            }
        
            AnsiConsole.MarkupLine("\nPress any key to continue...".Italic().AsInfo());
            AnsiConsole.Console.Input.ReadKey(true);
            AnsiConsole.Clear();
        }
    }
    
    private static MenuOption ShowMenu()
    {
        var prompt = new SelectionPrompt<MenuOption>()
            .Title("What would you like to search for? 🔎".AsPrimary())
            .PageSize(10)
            .AddChoices(MenuConfiguration.Options);

        return AnsiConsole.Prompt(prompt);
    }
}