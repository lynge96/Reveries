using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Handlers.Interfaces;
using Reveries.Console.Services.Interfaces;

namespace Reveries.Console.Services;

public class MenuOperationService : IMenuOperationService
{
    // Dictionary that maps each menu choice to its corresponding handler implementation
    private readonly IReadOnlyDictionary<MenuChoice, IMenuHandler> _handlers;
    
    // Initializes the service with all available menu handlers.
    public MenuOperationService(IEnumerable<IMenuHandler> handlers) =>
        _handlers = handlers.ToDictionary(h => h.MenuChoice);
    
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
}