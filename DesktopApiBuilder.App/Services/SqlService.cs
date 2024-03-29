using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class SqlService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string AppSettingsTemplatePath = "wwwroot\\templates\\api\\AppSettingsTemplate.txt";

    public static void SetupSqlConnection(string solutionName, string sqlServerName, string dbName)
    {
        try
        {
            var sqlConnection = $"Server={sqlServerName};Database={dbName};Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True";
            var fileContent = TemplateHelper.GetTemplateContent(AppSettingsTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\appsettings.json");

            sw.WriteLine(string.Format(fileContent, sqlConnection));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void AddMigration(string solutionName, string migrationName, bool applyMigration)
    {
        var commands = new List<string>()
        {
            $"cd {Path}\\{solutionName}",
            $"dotnet ef migrations add {migrationName} --project {solutionName}.DAL --startup-project {solutionName}.API"
        };

        if (applyMigration)
        {
            commands.Add($"dotnet ef database update --project {solutionName}.DAL --startup-project {solutionName}.API");
        }

        ProcessManager.ExecuteCmdCommands([.. commands]);
    }
}
