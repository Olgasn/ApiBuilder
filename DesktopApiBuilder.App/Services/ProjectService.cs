using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Helpers;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace DesktopApiBuilder.App.Services;

public static class ProjectService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string ProjectCreationCommandTemplate = "dotnet new {0} --name {1}.{2}";
    private const string AddProjectToSolutionCommandTemplate = "dotnet sln {0}.sln add {0}.{1}/{0}.{1}.csproj";
    private const string GoToProjectPathCommandTemplate = "cd {0}/{1}/{1}.{2}";

    public static void CreateProjects(string solutionName, ArchitectureType architectureType)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig($"{Path}\\myconfig.json");
        List<string> commands = GenerateExecutionCommands(solutionName, config);

        ProcessManager.ExecuteCmdCommands([.. commands]);

        CheckIfDirectoriesExist(solutionName, config);

        var apiProjectName = $"{solutionName}.{config?.Projects?.FirstOrDefault(p => p.Type == ProjectTypes.AspNetWebApi)?.Name}";
        var xmlPath = $"{Path}\\{solutionName}\\{apiProjectName}\\{apiProjectName}.csproj";

        XmlDocument doc = new();
        doc.Load(xmlPath);

        doc.SelectSingleNode("/Project/PropertyGroup/InvariantGlobalization").FirstChild.Value = "false";

        using XmlTextWriter xtw = new(xmlPath, Encoding.UTF8);
        doc.WriteContentTo(xtw);
    }

    private static List<string> GenerateExecutionCommands(string solutionName, SolutionConfig? config)
    {
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

        foreach (var project in config.Projects)
        {
            commands.Add(string.Format(GoToProjectPathCommandTemplate,
                Path,
                solutionName,
                project.Name));

            if (project.Type == ProjectTypes.ClassLibrary)
            {
                commands.Add("del \"Class1.cs\"");
            }
            else if (project.Type == ProjectTypes.AspNetWebApi)
            {
                commands.Add($"del \"{solutionName}.{project.Name}.http\"");
            }

            foreach (var dir in project.Directories)
            {
                if (!string.IsNullOrEmpty(dir.ParentPath))
                {
                    commands.Add(string.Format(GoToProjectPathCommandTemplate,
                        Path,
                        solutionName,
                        $"{project.Name}{dir.ParentPath}"));
                }

                commands.Add($"mkdir {dir.Name}");
            }
        }

        return commands;
    }

    private static void CheckIfDirectoriesExist(string solutionName, SolutionConfig? config)
    {
        List<string> commonPaths = [];

        config.Projects.ToList()
            .ForEach(p => commonPaths.AddRange(
                p.Directories.Where(d => string.IsNullOrEmpty(d.ParentPath))
                    .Select(d => $"{Path}/{solutionName}/{solutionName}.{p.Name}/{d.Name}")));

        while (true)
        {
            bool allExist = true;

            foreach (var path in commonPaths)
            {
                if (!Directory.Exists(path))
                {
                    allExist = false;
                    break;
                }
            }

            if (allExist) break;
        }
    }
}
