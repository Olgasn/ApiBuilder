﻿using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class SqlService
{
    private const string AppSettingsTemplatePath = "wwwroot\\templates\\api\\AppSettingsTemplate.txt";

    public static void SetupSqlConnection(SolutionSettingsModel solutionSettings, string sqlServerName, string dbName)
    {
        try
        {
            var sqlConnection = $"Server={sqlServerName};Database={dbName};Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True";
            var fileContent = TemplateHelper.GetTemplateContent(AppSettingsTemplatePath);

            SolutionConfig? config = ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);
            var apiProject = config?.Projects?.FirstOrDefault(p => p.Type == ProjectTypes.AspNetWebApi);
            var projectPath = ConfigHelper.GetProjectPath(config, apiProject, solutionSettings.SolutionName);

            StreamWriter sw = new($"{solutionSettings.SolutionPath}/{projectPath}/{solutionSettings.SolutionName}.{apiProject?.Name}/appsettings.json");

            sw.WriteLine(string.Format(fileContent, sqlConnection));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void AddMigration(SolutionSettingsModel solutionSettings, string migrationName, bool applyMigration)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);
        var apiProject = config?.Projects?.FirstOrDefault(p => p.Type == ProjectTypes.AspNetWebApi);
        var migrationProject = config?.Projects?.FirstOrDefault(p => p.Name == config?.MigrationsProjectName);

        var apiProjectPath = ConfigHelper.GetProjectPath(config, apiProject, solutionSettings.SolutionName);
        var migrationProjectPath = ConfigHelper.GetProjectPath(config, migrationProject, solutionSettings.SolutionName);

        var startupProjectName = $"{apiProjectPath}/{solutionSettings.SolutionName}.{apiProject?.Name}";
        var migrationsProjectName = $"{migrationProjectPath}/{solutionSettings.SolutionName}.{config?.MigrationsProjectName}";

        var commands = new List<string>()
        {
            $"cd {solutionSettings.SolutionPath}",
            $"dotnet ef migrations add {migrationName} --project {migrationsProjectName} --startup-project {startupProjectName}"
        };

        if (applyMigration)
        {
            commands.Add($"dotnet ef database update --project {migrationsProjectName} --startup-project {startupProjectName}");
        }

        ProcessManager.ExecuteCmdCommands([.. commands]);
    }
}
