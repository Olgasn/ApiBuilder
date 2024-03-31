using DesktopApiBuilder.App.Data.Constants;
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
            var projectName = $"{solutionSettings.SolutionName}.{config?.Projects?.FirstOrDefault(p => p.Type == ProjectTypes.AspNetWebApi)?.Name}";

            StreamWriter sw = new($"{solutionSettings.FullSolutionPath}\\{projectName}\\appsettings.json");

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
        var startupProjectName = $"{solutionSettings.SolutionName}.{config?.Projects?.FirstOrDefault(p => p.Type == ProjectTypes.AspNetWebApi)?.Name}";
        var migrationsProjectName = $"{solutionSettings.SolutionName}.{config?.MigrationsProjectName}";

        var commands = new List<string>()
        {
            $"cd {solutionSettings.FullSolutionPath}",
            $"dotnet ef migrations add {migrationName} --project {migrationsProjectName} --startup-project {startupProjectName}"
        };

        if (applyMigration)
        {
            commands.Add($"dotnet ef database update --project {migrationsProjectName} --startup-project {startupProjectName}");
        }

        ProcessManager.ExecuteCmdCommands([.. commands]);
    }
}
