﻿using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Data.Models;

public class SolutionSettingsModel
{
    public string SolutionPath { get; set; } = default!;
    public string SolutionName { get; set; } = default!;
    public ArchitectureType ArchitectureType { get; set; }

    public string FullSolutionPath => $"{SolutionPath}/{SolutionName}";
}
