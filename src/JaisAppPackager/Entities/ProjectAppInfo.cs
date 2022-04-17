using System.Text.Json.Serialization;
using CliFx.Exceptions;

namespace JaisAppPackager.Entities;

public record ProjectAppInfo
{
    [JsonPropertyName("appName")]
    public string? AppName { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("buildProject")]
    public string? BuildProject { get; set; }

    [JsonPropertyName("bundleId")]
    public string? BundleId { get; set; }

    public void Check()
    {
        var missingProperties = new List<string>();

        if (string.IsNullOrEmpty(AppName)) { missingProperties.Add(nameof(AppName)); }
        if (string.IsNullOrEmpty(Author)) { missingProperties.Add(nameof(Author)); }
        if (string.IsNullOrEmpty(Version)) { missingProperties.Add(nameof(Version)); }
        if (string.IsNullOrEmpty(BuildProject)) { missingProperties.Add(nameof(BuildProject)); }
        if (string.IsNullOrEmpty(BundleId)) { missingProperties.Add(nameof(BundleId)); }

        if (missingProperties.Count > 0)
        {
            string message = missingProperties.Count == 1 ?
                $"Required property \"{missingProperties.FirstOrDefault()}\" is missing in AppInfo.json" :
                $"Required properties \"{string.Join(", ", missingProperties)}\" are missing in AppInfo.json";

            throw new CommandException(message);
        }
    }
}