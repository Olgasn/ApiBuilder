using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models;

public class ProjectConfig
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
