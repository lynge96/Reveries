using Reveries.Application.Interfaces.Services;
using Reveries.Console.Interfaces;

namespace Reveries.Console.Services.Handlers;

public class SearchBookHandler : IMenuHandler
{
    private readonly IBookService _bookService;
    
    public SearchBookHandler(IBookService bookService)
    {
        _bookService = bookService;
    }
    
    public async Task HandleAsync()
    {
        // Implementation af bog søgning
    }
}
