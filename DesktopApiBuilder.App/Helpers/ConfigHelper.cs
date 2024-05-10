using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Configs;
using Microsoft.AspNetCore.Components.Forms;
using System.Text;
using System.Text.Json;

namespace DesktopApiBuilder.App.Helpers;

public static class ConfigHelper
{
    private const string ConfigsPath = "wwwroot\\defaultConfigs";

    public static SolutionConfig? GetSolutionConfig(ArchitectureType architectureType)
    {
        string absoluteConfigsPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\AppX\\", ConfigsPath);

        try
        {
            string json = File.ReadAllText($"{absoluteConfigsPath}\\configType{(int)architectureType}.json");
            return JsonSerializer.Deserialize<SolutionConfig>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public static async Task<SolutionConfig?> GetCustomSolutionConfig(IBrowserFile? browserFile)
    {
        if (browserFile is null) return null;

        try
        {
            var memoryStream = new MemoryStream();
            await browserFile.OpenReadStream().CopyToAsync(memoryStream);
            string json = Encoding.UTF8.GetString(memoryStream.ToArray());

            return JsonSerializer.Deserialize<SolutionConfig>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public static void SaveCustomSolutionConfig(string path, SolutionConfig? config)
    {
        try
        {
            var json = JsonSerializer.Serialize(config);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static string GetProjectPath(SolutionConfig? config, ProjectConfig? project, string solutionName)
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

    public static DirectoryConfig? FindByContentType(this IEnumerable<DirectoryConfig>? directories, 
        DirectoryContentType contentType, bool multipleContentTypes = false) => 
        multipleContentTypes
        ? directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(contentType.ToString()))
        : directories?.FirstOrDefault(d => d.ContentType == contentType.ToString());
}
