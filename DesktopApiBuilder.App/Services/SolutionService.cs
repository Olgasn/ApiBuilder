namespace DesktopApiBuilder.App.Services;

public static class SolutionService
{
    private const string Path = "C:\\D\\Projects\\test";

    public static void CreateSolution(string name)
    {
        ProcessManager.ExecuteCmdCommands([
            $"cd {Path}",
            $"mkdir {name}",
            $"cd {Path}\\{name}",
            $"dotnet new sln --name {name}"
        ]);
    }
}
