using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
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
            var option = ConsolePromptUtility.ShowSelectionPrompt(
                "What would you like to search for? 🔎", 
                MenuConfiguration.Options);
            
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
                AnsiConsole.MarkupLine($"{"Error:".Underline().AsError()} {Markup.Escape($"[{ex.GetType().Name}] {ex.Message}").Italic().AsWarning()}");
            }
        
            AnsiConsole.MarkupLine("Press any key to continue...".Italic().AsInfo());
            AnsiConsole.Console.Input.ReadKey(true);
            AnsiConsole.Clear();
        }
    }
}