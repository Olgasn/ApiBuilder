using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models.Configs;

public class DirectoryConfig
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("parentPath")]
    public string? ParentPath { get; set; }

    [JsonPropertyName("contentType")]
    public string? ContentType { get; set; }
}
