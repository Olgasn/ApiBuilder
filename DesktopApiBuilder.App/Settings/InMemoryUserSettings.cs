﻿using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Configs;

namespace DesktopApiBuilder.App.Settings;

public static class InMemoryUserSettings
{
    public static string SolutionPath { get; set; } = default!;
    public static string SolutionName { get; set; } = default!;
    public static ArchitectureType ArchitectureType { get; set; }
    public static IdType IdType { get; set; }
    public static SqlProviders SqlProvider { get; set; }
    public static SolutionConfig? CustomSolutionConfig { get; set; }
}
