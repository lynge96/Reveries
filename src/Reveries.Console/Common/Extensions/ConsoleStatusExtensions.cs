using Spectre.Console;

namespace Reveries.Console.Common.Extensions;

public static class ConsoleStatusExtensions
{
    public static async Task<(T Result, long ElapsedMs)> RunWithStatusAsync<T>(this IAnsiConsole console, Func<Task<T>> action, string spinnerStyle = ConsoleThemeExtensions.Secondary)
    {
        return await console.Status()
            .StartAsync<(T Result, long ElapsedMs)>("Searching".AsPrimary(), async ctx =>
            {
                ctx.Spinner(Spinner.Known.Default);
                ctx.SpinnerStyle(spinnerStyle);

                var timer = System.Diagnostics.Stopwatch.StartNew();
                var result = await action();
            
                return (result, timer.ElapsedMilliseconds);
            });
    }

    public static async Task<long> RunWithStatusAsync(this IAnsiConsole console, Func<Task> action, string spinnerStyle = ConsoleThemeExtensions.Secondary)
    {
        return await console.Status()
            .StartAsync<long>("Searching".AsPrimary(), async ctx =>
            {
                ctx.Spinner(Spinner.Known.Default);
                ctx.SpinnerStyle(spinnerStyle);

                var timer = System.Diagnostics.Stopwatch.StartNew();
                await action();
            
                return timer.ElapsedMilliseconds;
            });
    }
}
