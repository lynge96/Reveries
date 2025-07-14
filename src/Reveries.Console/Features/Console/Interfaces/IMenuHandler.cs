using Reveries.Console.Common.Models.Menu;

namespace Reveries.Console.Features.Console.Interfaces;

public interface IMenuHandler
{
    MenuChoice MenuChoice { get; }
    Task HandleAsync();
}