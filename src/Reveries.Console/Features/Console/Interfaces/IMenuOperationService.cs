using Reveries.Console.Common.Models.Menu;

namespace Reveries.Console.Features.Console.Interfaces;

/// <summary>
/// Defines operations for handling menu choices in the application.
/// </summary>
public interface IMenuOperationService
{
    /// <summary>
    /// Handles a specific menu choice by finding and executing the corresponding handler.
    /// </summary>
    /// <param name="choice">The menu choice to handle</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is found for the given choice</exception>
    Task HandleMenuChoiceAsync(MenuChoice choice);
}
