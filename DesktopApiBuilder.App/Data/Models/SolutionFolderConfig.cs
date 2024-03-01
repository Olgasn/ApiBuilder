using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models;

public class SolutionFolderConfig
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
