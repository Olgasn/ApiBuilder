using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models.Configs;

public class SolutionConfig
{
    [JsonPropertyName("solutionFolders")]
    public IEnumerable<SolutionFolderConfig>? SolutionFolders { get; set; }

    [JsonPropertyName("projects")]
    public IEnumerable<ProjectConfig>? Projects { get; set; }

    [JsonPropertyName("migrationsProjectName")]
    public string? MigrationsProjectName { get; set; }
}
