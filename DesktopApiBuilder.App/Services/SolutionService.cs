using DesktopApiBuilder.App.Data.Models;

namespace DesktopApiBuilder.App.Services;

public static class SolutionService
{
    public static void CreateSolution(SolutionSettingsModel solutionSettings)
    {
        ProcessManager.ExecuteCmdCommands([
            $"cd {solutionSettings.SolutionPath}",
            $"mkdir {solutionSettings.SolutionName}",
            $"cd {solutionSettings.FullSolutionPath}",
            $"dotnet new sln --name {solutionSettings.SolutionName}"
        ]);
    }
}
