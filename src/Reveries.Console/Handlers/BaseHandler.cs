using System.Net;
using Reveries.Console.Common.Extensions;
using Reveries.Console.Common.Models.Menu;
using Reveries.Console.Interfaces;
using Spectre.Console;

namespace Reveries.Console.Handlers;

public abstract class BaseHandler : IMenuHandler
{
    public abstract MenuChoice MenuChoice { get; }
    
    protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    
    public async Task HandleAsync(CancellationToken cancellationToken = default)
    {
        AnsiConsole.Clear();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));
        
        try
        {
            await ExecuteAsync(cts.Token);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            AnsiConsole.MarkupLine("No results found.".AsWarning());
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
        {
            AnsiConsole.MarkupLine("Access denied. Please verify that your API key is valid.".AsError());
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("The search was canceled due to timeout".AsWarning());
        }
    }
}
