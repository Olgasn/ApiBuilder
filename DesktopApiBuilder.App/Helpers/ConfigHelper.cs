using DesktopApiBuilder.App.Data.Models;
using System.Text.Json;

namespace DesktopApiBuilder.App.Helpers;

public static class ConfigHelper
{
    public static SolutionConfig? GetSolutionConfig(string path)
    {
        try
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SolutionConfig>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public static string GetProjectPath(SolutionConfig? config, ProjectConfig project, string solutionName)
    {
        var solutionFolderName = GetSolutionPathForProject(config, project);
        var projectPath = solutionName;

        if (!string.IsNullOrEmpty(solutionFolderName))
        {
            projectPath += $"/{solutionFolderName}";
        }

        return projectPath;
    }

    public static string GetSolutionPathForProject(SolutionConfig? config, ProjectConfig? project) => 
        config?.SolutionFolders?.FirstOrDefault(f => f.Id == project?.SolutionFolderId)?.Name ?? string.Empty;
}
