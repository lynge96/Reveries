
namespace Reveries.Console.Common.Extensions;

public static class ConsoleThemeExtensions
{
    public const string Header = "blueviolet";
    public const string Primary = "springgreen1";
    public const string Secondary = "deepskyblue1";
    public const string Success = "chartreuse2";
    public const string Warning = "yellow";
    public const string Error = "red";
    public const string Info = "grey";

    private static string Format(string? text, string color) => $"[{color}]{text}[/]";
    private static string StyleText(this string text, string style) => $"[{style}]{text}[/]";

    // Color styling for strings   
    public static string AsHeader(this string? text) => Format(text, Header);
    public static string AsPrimary(this string? text) => Format(text, Primary);
    public static string AsSecondary(this string? text) => Format(text, Secondary);
    public static string AsSuccess(this string? text) => Format(text, Success);
    public static string AsWarning(this string? text) => Format(text, Warning);
    public static string AsError(this string? text) => Format(text, Error);
    public static string AsInfo(this string? text) => Format(text, Info);
    
    // Color styling for integers
    public static string AsHeader(this int number) => Format(number.ToString(), Header);
    public static string AsPrimary(this int number) => Format(number.ToString(), Primary);
    public static string AsSecondary(this int number) => Format(number.ToString(), Secondary);
    public static string AsSuccess(this int number) => Format(number.ToString(), Success);
    public static string AsWarning(this int number) => Format(number.ToString(), Warning);
    public static string AsError(this int number) => Format(number.ToString(), Error);
    public static string AsInfo(this int number) => Format(number.ToString(), Info);

    // Text Styling for strings
    public static string Bold(this string text) => text.StyleText("b");
    public static string Italic(this string text) => text.StyleText("i");
    public static string Underline(this string text) => text.StyleText("u");
    
    // Text Styling for integers
    public static string Bold(this int number) => number.ToString().StyleText("b");
    public static string Italic(this int number) => number.ToString().StyleText("i");
    public static string Underline(this int number) => number.ToString().StyleText("u");
}