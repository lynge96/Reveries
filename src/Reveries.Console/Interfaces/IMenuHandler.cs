using Reveries.Console.Common.Models.Menu;

namespace Reveries.Console.Interfaces;

public interface IMenuHandler
{
    MenuChoice MenuChoice { get; }
    Task HandleAsync(CancellationToken cancellationToken = default);
}