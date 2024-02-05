using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models;

public class DirectoryConfig
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("parentPath")]
    public string? ParentPath { get; set; }
}
