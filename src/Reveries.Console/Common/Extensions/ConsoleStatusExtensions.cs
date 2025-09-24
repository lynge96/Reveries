using Spectre.Console;

namespace Reveries.Console.Common.Extensions;

public static class ConsoleStatusExtensions
{
    public static async Task<(T Result, long ElapsedMs)> RunWithStatusAsync<T>(this IAnsiConsole console, Func<Task<T>> action)
    {
        return await console.Status()
            .StartAsync<(T Result, long ElapsedMs)>("Searching".AsPrimary(), async ctx =>
            {
                ctx.Spinner(Spinner.Known.Default);
                ctx.SpinnerStyle(ConsoleThemeExtensions.Secondary);

                var timer = System.Diagnostics.Stopwatch.StartNew();
                var result = await action();
                
                return (result, timer.ElapsedMilliseconds);
            });
    }
}
