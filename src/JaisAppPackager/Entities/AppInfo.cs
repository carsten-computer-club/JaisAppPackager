using System.Text.Json.Serialization;

namespace JaisAppPackager.Entities;

public record AppInfo
{
    [JsonPropertyName("appName")]
    public string? AppName { get; set; }

    [JsonPropertyName("author")]
    public string? Author { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("buildProject")]
    public string? BuildProject { get; set; }
}