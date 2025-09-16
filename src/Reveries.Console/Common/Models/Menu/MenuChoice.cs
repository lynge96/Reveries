namespace Reveries.Console.Common.Models.Menu;

public enum MenuChoice
{
    // Main menu options
    ApiOperations,
    DatabaseOperations,
    // API operations
    SearchBook,
    SearchAuthor,
    SearchPublisher,
    // Database operations
    BooksInDatabase,
    BookSeries,
    
    Exit,
    Back
}