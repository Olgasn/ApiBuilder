using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class ProjectService
{
    // 0 - project type
    // 1 - solution name
    // 2 - project name
    private const string ProjectCreationCommandTemplate = "dotnet new {0} --name {1}.{2}";

    // 0 - solution name
    // 1 - project path
    // 2 - project name
    private const string AddProjectToSolutionCommandTemplate = "dotnet sln {0}.sln add {1}{0}.{2}/{0}.{2}.csproj";

    // 0 - path
    // 1 - project path
    // 2 - solution name
    // 3 - project name
    private const string GoToProjectPathCommandTemplate = "cd {0}/{1}/{2}.{3}";

    private const string ConfigsPath = "wwwroot\\defaultConfigs";

    public static void CreateProjects(string solutionName, string solutionPath, ArchitectureType architectureType)
    {
        string absoluteConfigsPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\AppX\\", ConfigsPath);

        SolutionConfig? config = ConfigHelper.GetSolutionConfig($"{absoluteConfigsPath}\\configType{(int)architectureType}.json");
        List<string> commands = GenerateExecutionCommands(solutionName, solutionPath, config);

        ProcessManager.ExecuteCmdCommands([.. commands]);

        CheckIfDirectoriesExist(solutionName, solutionPath, config);
    }

    private static List<string> GenerateExecutionCommands(string solutionName, string solutionPath, SolutionConfig? config)
    {
        List<string> commands = [];

        commands.Add($"cd {solutionPath}\\{solutionName}");

        foreach (var folder in config?.SolutionFolders ?? [])
        {
            commands.Add($"mkdir {folder.Name}");
        }

        commands.AddRange(GenerateProjectsCreationCommands(solutionName, solutionPath, config));
        commands.AddRange(GenerateProjectDirectoriesCreationCommands(solutionName, solutionPath, config));

        return commands;
    }

    private static List<string> GenerateProjectsCreationCommands(string solutionName, string solutionPath, SolutionConfig? config)
    {
        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var solutionFolderName = config?.SolutionFolders?
                .FirstOrDefault(f => f.Id == project.SolutionFolderId)?.Name ?? string.Empty;

            if (!string.IsNullOrEmpty(solutionFolderName))
            {
                commands.Add($"cd {solutionPath}\\{solutionName}\\{solutionFolderName}");
                solutionFolderName += "/";
            }
            else
            {
                commands.Add($"cd {solutionPath}\\{solutionName}");
            }

            commands.Add(string.Format(ProjectCreationCommandTemplate,
                project.Type,
                solutionName,
                project.Name));

            commands.Add($"cd {solutionPath}\\{solutionName}");

            commands.Add(string.Format(AddProjectToSolutionCommandTemplate,
                solutionName,
                solutionFolderName,
                project.Name));
        }

        return commands;
    }

    private static List<string> GenerateProjectDirectoriesCreationCommands(string solutionName, string solutionPath, SolutionConfig? config)
    {
        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var projectPath = ConfigHelper.GetProjectPath(config, project, solutionName);

            commands.Add(string.Format(GoToProjectPathCommandTemplate,
                solutionPath,
                projectPath,
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

            foreach (var dir in project?.Directories ?? [])
            {
                if (!string.IsNullOrEmpty(dir.ParentPath))
                {
                    commands.Add(string.Format(GoToProjectPathCommandTemplate,
                        solutionPath,
                        projectPath,
                        solutionName,
                        $"{project?.Name}{dir.ParentPath}"));
                }

                commands.Add($"mkdir {dir.Name}");
            }
        }

        return commands;
    }

    private static void CheckIfDirectoriesExist(string solutionName, string solutionPath, SolutionConfig? config)
    {
        List<string> commonPaths = [];

        config?.Projects?.ToList()
            .ForEach(p => commonPaths.AddRange(
                p?.Directories?.Where(d => string.IsNullOrEmpty(d?.ParentPath))
                    .Select(d => {
                        var solutionFolderName = config?.SolutionFolders?
                            .FirstOrDefault(f => f.Id == p.SolutionFolderId)?.Name ?? string.Empty;

                        if (!string.IsNullOrEmpty(solutionFolderName))
                        {
                            solutionFolderName += "/";
                        }

                        return $"{solutionPath}/{solutionName}/{solutionFolderName}{solutionName}.{p?.Name}/{d?.Name}";
                    }) ?? []));

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
