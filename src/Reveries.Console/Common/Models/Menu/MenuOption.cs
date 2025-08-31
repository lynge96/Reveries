namespace Reveries.Console.Common.Models.Menu;

public class MenuOption
{
    public MenuChoice Choice { get; }
    
    public string DisplayName { get; }
    
    public string Icon { get;  }
    
    public MenuOption(MenuChoice choice, string displayName, string icon)
    {
        Choice = choice;
        DisplayName = displayName;
        Icon = icon;
    }
    
    public override string ToString() => $"{Icon} {DisplayName}";
}