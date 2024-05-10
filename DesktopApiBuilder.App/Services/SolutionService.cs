using DesktopApiBuilder.App.Data.Models;

namespace DesktopApiBuilder.App.Services;

public static class SolutionService
{
    public static async Task CreateSolution(SolutionSettingsModel solutionSettings, CancellationToken ct)
    {
        await ProcessManager.ExecuteCmdCommands([
            $"cd /d {solutionSettings.SolutionPath}",
            $"mkdir {solutionSettings.SolutionName}",
            $"cd /d {solutionSettings.FullSolutionPath}",
            $"dotnet new sln --name {solutionSettings.SolutionName}"
        ], ct);
    }
}
