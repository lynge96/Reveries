using Reveries.Console.Interfaces;
using Reveries.Console.Models.Menu;
using Reveries.Console.Services.Handlers;

namespace Reveries.Console.Services;

public class MenuOperationService : IMenuOperationService
{
    private readonly IReadOnlyDictionary<MenuChoice, IMenuHandler> _handlers;

    public MenuOperationService(IEnumerable<IMenuHandler> handlers)
    {
        _handlers = handlers.ToDictionary(
            handler => GetMenuChoiceFromHandler(handler),
            handler => handler);
    }

    public async Task HandleMenuChoiceAsync(MenuChoice choice)
    {
        if (_handlers.TryGetValue(choice, out var handler))
        {
            await handler.HandleAsync();
        }
        else
        {
            throw new InvalidOperationException($"No handlers were found for that menu: {choice}");
        }
    }
    
    private static MenuChoice GetMenuChoiceFromHandler(IMenuHandler handler) =>
        handler switch
        {
            SearchBookHandler => MenuChoice.SearchBook,
            _ => throw new ArgumentException($"Unknown handler type: {handler.GetType().Name}")
        };

}