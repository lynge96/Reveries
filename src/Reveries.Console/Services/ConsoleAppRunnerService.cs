using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Services;

public class ConsoleAppRunnerService : IConsoleAppRunnerService
{
    private readonly IMenuOperationService _menuOperationService;
    private bool _isRunning = true;
    private readonly Stack<MenuOption[]> _menuStack = new();
    
    public ConsoleAppRunnerService(IMenuOperationService menuOperationService)
    {
        _menuOperationService = menuOperationService;
    }

    public async Task RunAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("Welcome to Reveries! 💫".Bold().AsHeader());
        
        _menuStack.Push(MenuConfiguration.MainMenu);
        
        while (_isRunning && _menuStack.Count > 0)
        {
            var currentMenu = _menuStack.Peek().ToList();

            var option = ConsolePromptUtility.ShowSelectionPrompt("What would you like to search for? 🔎", currentMenu);

            switch (option.Choice)
            {
                case MenuChoice.Exit:
                    _isRunning = false;
                    AnsiConsole.MarkupLine("\nGoodbye! ✨".Bold().AsPrimary());
                    return;
                case MenuChoice.ApiOperations:
                    _menuStack.Push(MenuConfiguration.ApiMenu);
                    continue;
                case MenuChoice.DatabaseOperations:
                    _menuStack.Push(MenuConfiguration.DatabaseMenu);
                    continue;
                case MenuChoice.Back:
                    _menuStack.Pop();
                    continue;
            }

            try
            {
                await _menuOperationService.HandleMenuChoiceAsync(option.Choice);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine(
                    $"{"Error:".Underline().AsError()} {Markup.Escape($"[{ex.GetType().Name}] {ex.Message}").Italic().AsWarning()}");
            }

            AnsiConsole.MarkupLine("Press any key to continue...".Italic().AsInfo());
            AnsiConsole.Console.Input.ReadKey(true);
            AnsiConsole.Clear();
        }
    }
}