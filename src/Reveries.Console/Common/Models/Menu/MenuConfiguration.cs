namespace Reveries.Console.Common.Models.Menu;

public static class MenuConfiguration
{
    public static readonly MenuOption[] MainMenu =
    [
        new(MenuChoice.ApiOperations, "API operations", "🌐"),
        new(MenuChoice.DatabaseOperations, "Database operations", "💾"),
        new(MenuChoice.Exit, "Exit application", "🚪")
    ];
    
    public static readonly MenuOption[] ApiMenu =
    [
        new(MenuChoice.SearchBook, "Books by ISBN(s) or name", "📖"),
        new(MenuChoice.SearchAuthor, "Authors by name", "👤"),
        new(MenuChoice.SearchPublisher, "Publishers by name", "🏢"),
        new(MenuChoice.Back, "Back", "↩️")
    ];
    
    public static readonly MenuOption[] DatabaseMenu =
    [
        new(MenuChoice.BooksInDatabase, "Get books from the database", "📚"),
        new(MenuChoice.Back, "Back", "↩️")
    ];

}
