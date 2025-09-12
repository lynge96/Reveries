namespace Reveries.Console.Common.Models.Menu;

public static class MenuConfiguration
{
    public static readonly MenuOption[] Options =
    [
        new(MenuChoice.SearchBook, "Books by ISBN(s) or name", "📖"),
        new(MenuChoice.SearchAuthor, "Authors by name", "👤"),
        new(MenuChoice.SearchPublisher, "Publishers by name", "🏢"),
        new(MenuChoice.BooksInDatabase, "Get books from the database", "💾"),
        new(MenuChoice.Exit, "Exit", "🚪")
    ];

}
