namespace Reveries.Console.Common.Models.Menu;

public static class MenuConfiguration
{
    public static readonly MenuOption[] Options =
    [
        new(MenuChoice.SearchBook, "Books by ISBN or name", "📖"),
        new(MenuChoice.SearchAuthor, "Authors by name", "👤"),
        new(MenuChoice.SearchPublisher, "Publishers by name", "🏢"),
        new(MenuChoice.Exit, "Exit", "🚪")
    ];

}
