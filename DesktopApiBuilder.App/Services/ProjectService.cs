using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Data.Models.Configs;
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
    private const string GoToProjectPathCommandTemplate = "cd /d {0}/{1}/{2}.{3}";

    public static async Task CreateProjects(SolutionSettingsModel solutionSettings, CancellationToken ct)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);
        List<string> commands = GenerateExecutionCommands(solutionSettings, config);

        await ProcessManager.ExecuteCmdCommands([.. commands], ct);

        CheckIfDirectoriesExist(solutionSettings, config, ct);
    }

    private static List<string> GenerateExecutionCommands(SolutionSettingsModel solutionSettings, SolutionConfig? config)
    {
        List<string> commands = [];

        commands.Add($"cd /d {solutionSettings.FullSolutionPath}");

        foreach (var folder in config?.SolutionFolders ?? [])
        {
            commands.Add($"mkdir {folder.Name}");
        }

        commands.AddRange(GenerateProjectsCreationCommands(solutionSettings, config));
        commands.AddRange(GenerateProjectDirectoriesCreationCommands(solutionSettings, config));

        return commands;
    }

    private static List<string> GenerateProjectsCreationCommands(SolutionSettingsModel solutionSettings, SolutionConfig? config)
    {
        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var solutionFolderName = config?.SolutionFolders?
                .FirstOrDefault(f => f.Id == project.SolutionFolderId)?.Name ?? string.Empty;

            if (!string.IsNullOrEmpty(solutionFolderName))
            {
                commands.Add($"cd /d {solutionSettings.FullSolutionPath}/{solutionFolderName}");
                solutionFolderName += "/";
            }
            else
            {
                commands.Add($"cd /d {solutionSettings.FullSolutionPath}");
            }

            commands.Add(string.Format(ProjectCreationCommandTemplate,
                project.Type,
                solutionSettings.SolutionName,
                project.Name));

            commands.Add($"cd /d {solutionSettings.FullSolutionPath}");

            commands.Add(string.Format(AddProjectToSolutionCommandTemplate,
                solutionSettings.SolutionName,
                solutionFolderName,
                project.Name));
        }

        return commands;
    }

    private static List<string> GenerateProjectDirectoriesCreationCommands(SolutionSettingsModel solutionSettings, SolutionConfig? config)
    {
        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var projectPath = ConfigHelper.GetProjectPath(config, project, solutionSettings.SolutionName);

            commands.Add(string.Format(GoToProjectPathCommandTemplate,
                solutionSettings.SolutionPath,
                projectPath,
                solutionSettings.SolutionName,
                project.Name));

            if (project.Type == ProjectTypes.ClassLibrary)
            {
                commands.Add("del \"Class1.cs\"");
            }
            else if (project.Type == ProjectTypes.AspNetWebApi)
            {
                commands.Add($"del \"{solutionSettings.SolutionName}.{project.Name}.http\"");
            }

            foreach (var dir in project.Directories ?? [])
            {
                if (!string.IsNullOrEmpty(dir.ParentPath))
                {
                    commands.Add(string.Format(GoToProjectPathCommandTemplate,
                        solutionSettings.SolutionPath,
                        projectPath,
                        solutionSettings.SolutionName,
                        $"{project.Name}{dir.ParentPath}"));
                }

                commands.Add($"mkdir {dir.Name}");
            }
        }

        return commands;
    }

    private static void CheckIfDirectoriesExist(SolutionSettingsModel solutionSettings, SolutionConfig? config, CancellationToken ct)
    {
        List<string> commonPaths = [];

        config?.Projects?.ToList()
            .ForEach(p => commonPaths.AddRange(
                p?.Directories?.Where(d => string.IsNullOrEmpty(d?.ParentPath))
                    .Select(d =>
                    {
                        var solutionFolderName = config?.SolutionFolders?
                            .FirstOrDefault(f => f.Id == p.SolutionFolderId)?.Name ?? string.Empty;

                        if (!string.IsNullOrEmpty(solutionFolderName))
                        {
                            solutionFolderName += "/";
                        }

                        return
                            $"{solutionSettings.FullSolutionPath}/{solutionFolderName}{solutionSettings.SolutionName}.{p?.Name}/{d?.Name}";
                    }) ?? []));

        while (!commonPaths.All(Directory.Exists))
        {
            ct.ThrowIfCancellationRequested();
        }
    }
}
