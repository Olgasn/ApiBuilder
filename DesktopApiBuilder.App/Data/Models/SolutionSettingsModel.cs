﻿using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Configs;

namespace DesktopApiBuilder.App.Data.Models;

public class SolutionSettingsModel
{
    public string SolutionPath { get; set; } = default!;
    public string SolutionName { get; set; } = default!;
    public ArchitectureType ArchitectureType { get; set; }
    public IdType IdType { get; set; }
    public SqlProviders SqlProvider { get; set; }
    public SolutionConfig? CustomSolutionConfig { get; set; }

    public string FullSolutionPath => $"{SolutionPath}/{SolutionName}";
}
