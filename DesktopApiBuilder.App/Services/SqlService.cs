using DesktopApiBuilder.App.Helpers;

namespace DesktopApiBuilder.App.Services;

public static class SqlService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string AppSettingsTemplatePath = "wwwroot\\templates\\api\\AppSettingsTemplate.txt";

    public static void SetupSqlConnection(string solutionName, string sqlConnection)
    {
        try
        {
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
}
