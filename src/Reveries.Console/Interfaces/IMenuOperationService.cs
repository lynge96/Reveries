using Reveries.Console.Models.Menu;

namespace Reveries.Console.Interfaces;

public interface IMenuOperationService
{
    Task HandleMenuChoiceAsync(MenuChoice choice);
}