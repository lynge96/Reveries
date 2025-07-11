using Reveries.Console.Common.Models.Menu;

namespace Reveries.Console.Features.Console.Interfaces;

public interface IMenuOperationService
{
    Task HandleMenuChoiceAsync(MenuChoice choice);
}