using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Services;

public static class ProjectService
{
    private const string Path = "C:\\D\\Projects\\test";

    public static void CreateProjects(string solutionName, ArchitectureType architectureType)
    {
        if (architectureType == ArchitectureType.ThreeLayered)
        {
            ProcessManager.ExecuteCmdCommands([
                $"cd {Path}\\{solutionName}",
                $"dotnet new {ProjectTemplates.AspNetWebApi} --name {solutionName}.API",
                $"dotnet new {ProjectTemplates.ClassLibrary} --name {solutionName}.BLL",
                $"dotnet new {ProjectTemplates.ClassLibrary} --name {solutionName}.DAL",
                $"dotnet sln {solutionName}.sln add {solutionName}.API/{solutionName}.API.csproj",
                $"dotnet sln {solutionName}.sln add {solutionName}.BLL/{solutionName}.BLL.csproj",
                $"dotnet sln {solutionName}.sln add {solutionName}.DAL/{solutionName}.DAL.csproj",
                // DAL
                $"cd {Path}\\{solutionName}\\{solutionName}.DAL",
                "del \"Class1.cs\"",
                "mkdir Entities",
                "mkdir Repositories",
                $"cd {Path}\\{solutionName}\\{solutionName}.DAL\\Repositories",
                "mkdir Abstractions",
                // BLL
                $"cd {Path}\\{solutionName}\\{solutionName}.BLL",
                "del \"Class1.cs\"",
                "mkdir Dtos",
                "mkdir Services",
                $"cd {Path}\\{solutionName}\\{solutionName}.BLL\\Services",
                "mkdir Abstractions",
                // API
                $"cd {Path}\\{solutionName}\\{solutionName}.API",
                "mkdir Controllers",
            ]);

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
                   )
            {
                continue;
            }
        }
    }
}
