using Reveries.Console.Common.Models.Menu;

namespace Reveries.Console.Features.Handlers.Interfaces;

public interface IMenuHandler
{
    MenuChoice MenuChoice { get; }
    Task HandleAsync(CancellationToken cancellationToken = default);
}