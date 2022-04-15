using System.IO.Compression;
using System.Text.Json;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using JaisAppPackager.Entities;
using JaisAppPackager.Services.Shell;

namespace JaisAppPackager.Commands;

[Command("package", Description = "Packages a JAIS app.")]
public class PackageCommand : ICommand
{
    private readonly IShellService _shellService;
    private IConsole _console = null!;

    [CommandOption("version", 'v')]
    public string? Version { get; set; }

    [CommandOption("output", 'o')]
    public string? OutputDirectory { get; set; }


    public PackageCommand(IShellService shellService)
    {
        _shellService = shellService;
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        _console = console;

        string[] files = Directory.GetFiles(Directory.GetCurrentDirectory());

        if (files.FirstOrDefault(path => Path.GetFileName(path) == "AppInfo.json") == null)
        {
            DisplayError("AppInfo.json file not found.\nEnsure you are in the right directory");
            return;
        }

        var appInfo = JsonSerializer.Deserialize<AppInfo>(await File.ReadAllTextAsync("AppInfo.json"));

        if (appInfo == null)
        {
            DisplayError("Error while trying to parse AppInfo.json");
            return;
        }

        string version = Version ?? appInfo.Version ?? "0.0.0";
        string outputDirectory = OutputDirectory ?? "build";

        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, true);
        }

        (int ExitCode, string? Output, string? ErrorOutput, Exception? Exception) result = await _shellService.Execute(
            "dotnet publish " +
            "-c Release " +
            $"-o {outputDirectory}/raw " +
            "-r linux-arm " +
            "-p:PublishReadyToRun=true " +
            "-p:PublishTrimmed=true " +
            "--self-contained true " +
            $"-p:PackageVersion={version} " +
            "-p:IncludeNativeLibrariesForSelfExtract=true " +
            $"{appInfo.BuildProject}",
            OutputReceived,
            ErrorOutputReceived);

        var memoryStream = new MemoryStream();
        using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true);

        string[] buildFiles = Directory.GetFiles("build/raw");

        archive.CreateEntryFromFile("AppInfo.json", "AppInfo.json");

        foreach (string filePath in buildFiles)
        {
            string fileName = Path.GetFileName(filePath);

            archive.CreateEntryFromFile(filePath, $"bin/{fileName}");
        }

        archive.Dispose();


        string zipFilePath = $"{outputDirectory}/{appInfo.AppName}-{version}.jaisapp";

        FileStream stream = File.OpenWrite(zipFilePath);
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(stream);

        console.ForegroundColor = ConsoleColor.Green;
        await console.Output.WriteLineAsync($"\nSuccessfully packaged app at {zipFilePath}");
        console.ResetColor();
    }

    private void ErrorOutputReceived(string message)
    {
        DisplayError(message);
    }

    private void OutputReceived(string message)
    {
        _console.Output.WriteLine(message);
    }

    private void DisplayError(string message)
    {
        _console.ForegroundColor = ConsoleColor.Red;
        _console.Output.WriteLine(message);
        _console.ResetColor();
    }
}