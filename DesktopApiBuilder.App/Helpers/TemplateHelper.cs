namespace DesktopApiBuilder.App.Helpers;

public static class TemplateHelper
{
    public static string GetTemplateContent(string templateFileName)
    {
        string absoluteTemplatesPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\AppX\\", templateFileName);

        try
        {
            StreamReader sr = new(absoluteTemplatesPath);
            var fileContent = sr.ReadToEnd();
            sr.Close();

            return fileContent;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return string.Empty;
        }
    }
}
