using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Features.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Features.Console.Services;

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
        
            AnsiConsole.MarkupLine("Press any key to continue...".Italic().AsInfo());
            AnsiConsole.Console.Input.ReadKey(true);
            AnsiConsole.Clear();
        }
    }
    
    private static MenuOption ShowMenu()
    {
        var prompt = new SelectionPrompt<MenuOption>()
            .Title("What would you like to search for? 🔎".AsPrimary())
            .PageSize(10)
            .AddChoices(MenuConfiguration.Options)
            .HighlightStyle(ConsoleThemeExtensions.Secondary);

        return AnsiConsole.Prompt(prompt);
    }
}