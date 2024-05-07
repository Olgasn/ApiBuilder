using DesktopApiBuilder.App.Data.Models;

namespace DesktopApiBuilder.App.Services;

public static class SolutionService
{
    public static void CreateSolution(SolutionSettingsModel solutionSettings)
    {
        ProcessManager.ExecuteCmdCommands([
            $"cd /d {solutionSettings.SolutionPath}",
            $"mkdir {solutionSettings.SolutionName}",
            $"cd /d {solutionSettings.FullSolutionPath}",
            $"dotnet new sln --name {solutionSettings.SolutionName}"
        ]);
    }
}
