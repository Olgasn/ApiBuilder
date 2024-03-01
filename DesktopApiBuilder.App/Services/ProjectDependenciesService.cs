using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class ProjectDependenciesService
{
    private const string Path = "C:\\D\\Projects\\test";

    // 0 - path
    // 1 - project path
    // 2 - solution name
    // 3 - project name
    private const string GoToProjectPathCommandTemplate = "cd {0}/{1}/{2}.{3}";

    public static void AddProjectReferences(string solutionName)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig($"{Path}\\myconfig0.json");

        List<string> commands = [];

        foreach (var project in config?.Projects ?? [])
        {
            var projectPath = ConfigHelper.GetProjectPath(config, project, solutionName);

            commands.Add(string.Format(GoToProjectPathCommandTemplate,
                Path,
                projectPath,
                solutionName,
                project.Name));

            foreach (var package in project?.Dependencies?.Packages ?? [])
            {
                commands.Add($"dotnet add package {package}");
            }

            commands.Add($"cd {Path}\\{solutionName}");

            foreach (var projectRef in project?.Dependencies?.ProjectReferences ?? [])
            {
                var relatedProject = config?.Projects?.FirstOrDefault(p => p.Name == projectRef);

                if (relatedProject is null) continue;

                var projectSolutionPath = ConfigHelper.GetSolutionPathForProject(config, project);
                var relatedProjectSolutionPath = ConfigHelper.GetSolutionPathForProject(config, relatedProject);
                var projectFullName = $"{solutionName}.{project?.Name}";
                var relatedProjectFullName = $"{solutionName}.{relatedProject.Name}";
                
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
