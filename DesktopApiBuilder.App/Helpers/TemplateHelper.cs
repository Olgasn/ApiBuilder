using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Helpers;

public static class TemplateHelper
{
    public static string GetTemplateContent(string templateFilePath)
    {
        string absoluteTemplatesPath = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\AppX\\", templateFilePath);

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

    public static string GetTemplateContent(DirectoryContentType contentType)
    {
        string absoluteTemplatesPath = AppDomain.CurrentDomain.BaseDirectory.Replace(
            "bin\\Debug\\net8.0-windows10.0.19041.0\\win10-x64\\AppX\\", GetTemplatePath(contentType));

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

    private static string GetTemplatePath(DirectoryContentType contentType) => 
        _ = contentType switch
        {
            DirectoryContentType.EntityClass => TemplatePaths.EntityTemplatePath,
            DirectoryContentType.RepositoryClass => TemplatePaths.RepositoryTemplatePath,
            DirectoryContentType.RepositoryInterface => TemplatePaths.RepositoryInterfaceTemplatePath,
            DirectoryContentType.DbContext => TemplatePaths.DbContextTemplatePath,
            DirectoryContentType.DtoClass => TemplatePaths.DtoTemplatePath,
            DirectoryContentType.MappingProfile => TemplatePaths.MappingProfileTemplatePath,
            DirectoryContentType.ServiceClass => TemplatePaths.ServiceTemplatePath,
            DirectoryContentType.ServiceInterface => TemplatePaths.ServiceInterfaceTemplatePath,
            DirectoryContentType.ProgramClass => TemplatePaths.ProgramClassTemplatePath,
            DirectoryContentType.Controller => TemplatePaths.ControllerTemplatePath,
            DirectoryContentType.ServiceExtensions => TemplatePaths.ServiceExtensionsTemplatePath,
            _ => throw new NotImplementedException(),
        };
}
