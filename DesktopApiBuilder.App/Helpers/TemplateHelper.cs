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
            DirectoryContentType.DbContext => throw new NotImplementedException(),
            DirectoryContentType.DtoClass => TemplatePaths.DtoTemplatePath,
            DirectoryContentType.MappingProfile => throw new NotImplementedException(),
            DirectoryContentType.ServiceClass => throw new NotImplementedException(),
            DirectoryContentType.ServiceInterface => throw new NotImplementedException(),
            DirectoryContentType.ProgramClass => throw new NotImplementedException(),
            DirectoryContentType.Controller => throw new NotImplementedException(),
            DirectoryContentType.ServiceExtensionsClass => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };
}
