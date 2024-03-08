namespace DesktopApiBuilder.App.Services;

public static class SolutionService
{
    public static void CreateSolution(string name, string path)
    {
        ProcessManager.ExecuteCmdCommands([
            $"cd {path}",
            $"mkdir {name}",
            $"cd {path}\\{name}",
            $"dotnet new sln --name {name}"
        ]);
    }
}
