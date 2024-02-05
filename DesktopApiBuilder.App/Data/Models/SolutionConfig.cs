using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models;

public class SolutionConfig
{
    [JsonPropertyName("projects")]
    public IEnumerable<ProjectConfig>? Projects { get; set; }
}
