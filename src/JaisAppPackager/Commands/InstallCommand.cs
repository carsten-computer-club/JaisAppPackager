using System.IO.Compression;
using System.Net;
using System.Text.Json;
using CliFx;
using CliFx.Attributes;
using CliFx.Exceptions;
using CliFx.Infrastructure;
using JaisAppPackager.Entities;
using JaisAppPackager.Extensions;

namespace JaisAppPackager.Commands;

[Command("install", Description = "Install a jaisapp over the air.")]
public class InstallCommand : ICommand
{
    [CommandParameter(1, Name = "File", Description = "The .jaisapp file.", IsRequired = true)]
    public string FilePath { get; set; } = null!;

    [CommandParameter(2, Name = "IpAddress", Description = "The ip address of the target JAIS system.", IsRequired = true)]
    public string IpAddress { get; set; } = null!;

    private void ProgressTracker(Stream streamToTrack, ref bool keepTracking)
    {
        Console.ForegroundColor = ConsoleColor.Gray;

        int previousProgress = -1;
        while (keepTracking)
        {
            var progress = (int) Math.Round(100 * (streamToTrack.Position / (double) streamToTrack.Length));

            if (progress != previousProgress)
            {
                Console.Write($"\rProgress: {progress}%");
            }

            previousProgress = progress;

            Thread.Sleep(100);
        }

        Console.ResetColor();
    }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        if (!File.Exists(FilePath))
        {
            throw new CommandException($"File {FilePath} does not exist.");
        }

        await using FileStream fileStream = File.OpenRead(FilePath);
        await using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        fileStream.Position = 0;

        using var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read);

        ZipArchiveEntry? appInfoEntry = zipArchive.Entries.FirstOrDefault(archiveEntry => archiveEntry.Name == "AppInfo.json");

        if (appInfoEntry == null)
        {
            throw new CommandException("Could not find AppInfo.json. The app may be packaged in a wrong way.");
        }

        await using Stream appInfoStream = appInfoEntry.Open();
        var appInfo = await JsonSerializer.DeserializeAsync<PackagedAppInfo>(appInfoStream);

        if (appInfo == null)
        {
            throw new CommandException("Could not read AppInfo.json.");
        }

        string fileName = Path.GetFileName(FilePath);

        using var client = new HttpClient();
        client.Timeout = new TimeSpan(0, 0, 0, 0, -1);

        // var authentication = $"Bearer {await _authenticationService.GetToken()}";
        // client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse(authentication);

        client.DefaultRequestHeaders.Add("fileName", fileName);
        client.DefaultRequestHeaders.Add("appName", appInfo.AppName);
        client.DefaultRequestHeaders.Add("appVersion", appInfo.Version);
        client.DefaultRequestHeaders.Add("author", appInfo.Author);
        client.DefaultRequestHeaders.Add("Transfer-Encoding", "chunked");

        var uri = new Uri($"http://{IpAddress}:5408/api/v1/apps/upload");
        using var fileStreamContent = new StreamContent(fileStream);

        await console.Output.WriteLineAsync("Transferring app...");

        var keepTracking = true;

        // ReSharper disable once AccessToDisposedClosure
        // ReSharper disable once AccessToModifiedClosure
        new Task(() => { ProgressTracker(fileStream, ref keepTracking); }).Start();
        using HttpResponseMessage response = await client.PostAsync(uri, fileStreamContent);
        keepTracking = false;

        HttpStatusCode responseCode = response.StatusCode;
        string responseMessage = await response.Content.ReadAsStringAsync();

        switch (responseCode)
        {
            case HttpStatusCode.OK:
            {
                console.ForegroundColor = ConsoleColor.Green;
                await console.Output.WriteLineAsync("\rSuccessfully installed app.");
                console.ResetColor();

                return;
            }

            case HttpStatusCode.Forbidden:
            {
                console.ForegroundColor = ConsoleColor.Red;
                await console.Output.WriteLineAsync("\rThe system declined the installation request.");
                console.ResetColor();

                return;
            }

            default:
                throw new CommandException($"\rAn error occured while trying to upload {FilePath} to http://{IpAddress}:5408\nStatus code: {responseCode}\n{responseMessage}");
        }
    }
}