using Reveries.Application.Interfaces.Services;
using Reveries.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Services;

public class ConsoleAppRunner : IConsoleAppRunner
{
    private readonly IBookService _bookService;
    
    public ConsoleAppRunner(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    public async Task RunAsync()
    {
        var book = await _bookService.GetBookByIsbnAsync("9780804139021");
        
        AnsiConsole.WriteLine(book!.ToString());
    }
}