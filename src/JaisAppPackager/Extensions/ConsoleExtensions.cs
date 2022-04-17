using CliFx.Infrastructure;

namespace JaisAppPackager.Extensions;

public static class ConsoleExtensions
{
    public static void WriteLineError(this IConsole console, string message)
    {
        console.ForegroundColor = ConsoleColor.Red;
        console.Output.WriteLine(message);
        console.ResetColor();
    }
}