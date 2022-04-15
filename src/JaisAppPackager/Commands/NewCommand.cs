using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using JaisAppPackager.Services.Shell;

namespace JaisAppPackager.Commands;

[Command("new", Description = "Create a new JAIS app.")]
public class NewCommand : ICommand
{
    private readonly IShellService _shellService;

    [CommandParameter(1, Name = "Name", Description = "The name of your app", IsRequired = true)]
    public string? Name { get; set; }

    [CommandOption("output", 'o', Description = "Output directory. Default: Creates a new directory with the name of this app")]
    public string? OutputDirectory { get; set; }

    public NewCommand(IShellService shellService)
    {
        _shellService = shellService;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        await console.Output.WriteLineAsync($"Creating project {Name}\n");

        if (Name != null)
        {
            string outputDirectory = OutputDirectory ?? Name;
            await _shellService.Execute($"dotnet new jaisapp --name {Name} --output {outputDirectory}", OutputReceived, ErrorOutputReceived);
        }
    }

    private static void ErrorOutputReceived(string obj)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(obj);
        Console.ResetColor();
    }

    private static void OutputReceived(string obj)
    {
        Console.WriteLine(obj);
    }
}