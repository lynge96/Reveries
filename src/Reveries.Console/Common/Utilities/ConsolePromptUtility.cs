using Reveries.Console.Common.Extensions;
using Spectre.Console;

namespace Reveries.Console.Common.Utilities;

public static class ConsolePromptUtility
{
    public static string GetUserInput(string promptText, string? promptStyle = null)
    {
        var prompt = new TextPrompt<string>(promptText.AsPrimary());
    
        if (promptStyle != null)
        {
            prompt.PromptStyle(promptStyle);
        }
        else
        {
            prompt.PromptStyle($"{ConsoleThemeExtensions.Secondary}");
        }
    
        return AnsiConsole.Prompt(prompt);
    }
    
    public static T ShowSelectionPrompt<T>(string title, IEnumerable<T> choices, int pageSize = 10) where T : notnull
    {
        var prompt = new SelectionPrompt<T>()
            .Title(title.AsPrimary())
            .PageSize(pageSize)
            .AddChoices(choices)
            .HighlightStyle(ConsoleThemeExtensions.Secondary);

        return AnsiConsole.Prompt(prompt);
    }

}
