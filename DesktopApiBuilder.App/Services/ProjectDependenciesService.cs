using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class ProjectDependenciesService
{
    // 0 - path
    // 1 - project path
    // 2 - solution name
    // 3 - project name
    private const string GoToProjectPathCommandTemplate = "cd {0}/{1}/{2}.{3}";

    public static void AddProjectReferences(SolutionSettingsModel solutionSettings)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);

        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var projectPath = ConfigHelper.GetProjectPath(config, project, solutionSettings.SolutionName);

            commands.Add(string.Format(GoToProjectPathCommandTemplate,
                solutionSettings.SolutionPath,
                projectPath,
                solutionSettings.SolutionName,
                project.Name));

            foreach (var package in project?.Dependencies?.Packages ?? [])
            {
                commands.Add($"dotnet add package {package}");
            }

            commands.Add($"cd {solutionSettings.FullSolutionPath}");

            foreach (var projectRef in project?.Dependencies?.ProjectReferences ?? [])
            {
                var relatedProject = config?.Projects?.FirstOrDefault(p => p.Name == projectRef);

                if (relatedProject is null) continue;

                var projectSolutionPath = ConfigHelper.GetSolutionPathForProject(config, project);
                var relatedProjectSolutionPath = ConfigHelper.GetSolutionPathForProject(config, relatedProject);
                var projectFullName = $"{solutionSettings.SolutionName}.{project?.Name}";
                var relatedProjectFullName = $"{solutionSettings.SolutionName}.{relatedProject.Name}";
                
                if (!string.IsNullOrEmpty(projectSolutionPath))
                {
                    projectSolutionPath += "/";
                }

                if (!string.IsNullOrEmpty(relatedProjectSolutionPath))
                {
                    relatedProjectSolutionPath += "/";
                }

                commands.Add($"dotnet add {projectSolutionPath}{projectFullName}/{projectFullName}.csproj reference {relatedProjectSolutionPath}{relatedProjectFullName}/{relatedProjectFullName}.csproj");
            }
        }

        ProcessManager.ExecuteCmdCommands([.. commands]);
    }
}
