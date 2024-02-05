using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class ProjectService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string ProjectCreationCommandTemplate = "dotnet new {0} --name {1}.{2}";
    private const string AddProjectToSolutionCommandTemplate = "dotnet sln {0}.sln add {0}.{1}/{0}.{1}.csproj";

    public static void CreateProjects(string solutionName, ArchitectureType architectureType)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig($"{Path}\\myconfig.json");

        List<string> commands = [];

        commands.Add($"cd {Path}\\{solutionName}");

        foreach (var project in config.Projects)
        {
            commands.Add(string.Format(ProjectCreationCommandTemplate, 
                project.Type,
                solutionName,
                project.Name));

            commands.Add(string.Format(AddProjectToSolutionCommandTemplate,
                solutionName,
                project.Name));
        }

        commands.AddRange(
            [$"cd {Path}\\{solutionName}\\{solutionName}.DAL",
            "del \"Class1.cs\"",
            "mkdir Entities",
            "mkdir Repositories",
            $"cd {Path}\\{solutionName}\\{solutionName}.DAL\\Repositories",
            "mkdir Abstractions",

            $"cd {Path}\\{solutionName}\\{solutionName}.BLL",
            "del \"Class1.cs\"",
            "mkdir Dtos",
            "mkdir Services",
            $"cd {Path}\\{solutionName}\\{solutionName}.BLL\\Services",
            "mkdir Abstractions",

            $"cd {Path}\\{solutionName}\\{solutionName}.API",
            $"del \"{solutionName}.API.http\"",
            "mkdir Controllers",
            "mkdir Extensions"]);

        ProcessManager.ExecuteCmdCommands([.. commands]);

        var dalPath = $"{Path}/{solutionName}/{solutionName}.DAL/";
        var bllPath = $"{Path}/{solutionName}/{solutionName}.BLL/";
        var apiPath = $"{Path}/{solutionName}/{solutionName}.API/";

        while (!Directory.Exists($"{dalPath}Entities")
               || !Directory.Exists($"{dalPath}Repositories")
               || !Directory.Exists($"{dalPath}Repositories/Abstractions")
               || !Directory.Exists($"{bllPath}Dtos")
               || !Directory.Exists($"{bllPath}Services")
               || !Directory.Exists($"{bllPath}Services/Abstractions")
               || !Directory.Exists($"{apiPath}Controllers")
               || !Directory.Exists($"{apiPath}Extensions"))
        {
            continue;
        }
    }
}
