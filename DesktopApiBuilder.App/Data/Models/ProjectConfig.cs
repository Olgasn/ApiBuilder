using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models;

public class ProjectConfig
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("solutionFolderId")]
    public int SolutionFolderId { get; set; }

    [JsonPropertyName("directories")]
    public IEnumerable<DirectoryConfig>? Directories { get; set; }

    [JsonPropertyName("dependencies")]
    public ProjectDependenciesConfig? Dependencies { get; set; }
}
