using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Common.Utilities;
using Reveries.Console.Services.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Services;

public class ConsoleAppRunnerService : IConsoleAppRunnerService
{
    private readonly IMenuOperationService _menuOperationService;
    private bool _isRunning = true;
    private MenuOption[] _currentMenu = MenuConfiguration.MainMenu;

    public ConsoleAppRunnerService(IMenuOperationService menuOperationService)
    {
        _menuOperationService = menuOperationService;
    }

    public async Task RunAsync()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("Welcome to Reveries! 💫".Bold().AsHeader());
        
        while (_isRunning)
        {
            var option = ConsolePromptUtility.ShowSelectionPrompt("What would you like to search for? 🔎", _currentMenu);

            switch (option.Choice)
            {
                case MenuChoice.Exit:
                    _isRunning = false;
                    AnsiConsole.MarkupLine("\nGoodbye! ✨".Bold().AsPrimary());
                    return;
                case MenuChoice.ApiOperations:
                    _currentMenu = MenuConfiguration.ApiMenu;
                    continue;
                case MenuChoice.DatabaseOperations:
                    _currentMenu = MenuConfiguration.DatabaseMenu;
                    continue;
                case MenuChoice.Back:
                    _currentMenu = MenuConfiguration.MainMenu;
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