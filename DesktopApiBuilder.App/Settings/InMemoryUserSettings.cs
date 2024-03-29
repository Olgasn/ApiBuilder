﻿using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Settings;

public static class InMemoryUserSettings
{
    public static string SolutionPath { get; set; } = default!;
    public static string SolutionName { get; set; } = default!;
    public static ArchitectureType ArchitectureType { get; set; }
}
