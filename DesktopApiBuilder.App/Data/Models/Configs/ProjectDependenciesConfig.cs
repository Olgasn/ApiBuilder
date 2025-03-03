﻿using System.Text.Json.Serialization;

namespace DesktopApiBuilder.App.Data.Models.Configs;

public class ProjectDependenciesConfig
{
    [JsonPropertyName("packages")]
    public IEnumerable<string>? Packages { get; set; }

    [JsonPropertyName("projectReferences")]
    public IEnumerable<string>? ProjectReferences { get; set; }
}
