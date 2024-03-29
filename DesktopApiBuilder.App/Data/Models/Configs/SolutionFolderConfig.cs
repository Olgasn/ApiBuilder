using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models.Configs;

public class SolutionFolderConfig
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
