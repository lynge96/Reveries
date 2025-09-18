using Reveries.Console.Common.Extensions;
using Spectre.Console;

namespace Reveries.Console.Common.Utilities;

public static class ConsolePromptUtility
{
    public static string GetUserInput(string promptText)
    {
        var prompt = new TextPrompt<string>(promptText.AsPrimary());
        
        prompt.PromptStyle($"{ConsoleThemeExtensions.Secondary}");
    
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

    public static List<T> ShowMultiSelectionPrompt<T>(string title, IEnumerable<T> choices, int pageSize = 10) where T : notnull
    {
        var indexedChoices = choices
            .Select((choice, index) => new { Index = index + 1, Value = choice })
            .ToList();
        
        var prompt = new MultiSelectionPrompt<string>()
            .Title(title.AsPrimary())
            .PageSize(pageSize)
            .NotRequired()
            .HighlightStyle(ConsoleThemeExtensions.Secondary)
            .InstructionsText("Press <space> to select, <enter> to confirm".AsInfo().Italic());

        if (indexedChoices.Count != 0)
        {
            prompt.AddChoiceGroup("All options",
                indexedChoices.Select(c => $"{c.Index}. {c.Value}").ToArray());
        }
        
        var selected = AnsiConsole.Prompt(prompt);
        
        return selected
            .Select(s =>
            {
                var idx = int.Parse(s.Split('.')[0]) - 1;
                return indexedChoices[idx].Value;
            })
            .ToList();
    }
    
    public static bool ShowYesNoPrompt(string title)
    {
        var choice = new SelectionPrompt<string>()
            .Title(title.AsPrimary())
            .AddChoices("Yes", "No")
            .HighlightStyle(ConsoleThemeExtensions.Secondary);

        var result = AnsiConsole.Prompt(choice);
        return result == "Yes";
    }
}
