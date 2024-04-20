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
            DirectoryContentType.DtoForCreationClass => TemplatePaths.DtoForCreationTemplatePath,
            DirectoryContentType.DtoForUpdateClass => TemplatePaths.DtoForUpdateTemplatePath,
            DirectoryContentType.MappingProfile => TemplatePaths.MappingProfileTemplatePath,
            DirectoryContentType.ServiceClass => TemplatePaths.ServiceTemplatePath,
            DirectoryContentType.ServiceInterface => TemplatePaths.ServiceInterfaceTemplatePath,
            DirectoryContentType.ProgramClass => TemplatePaths.ProgramClassTemplatePath,
            DirectoryContentType.ProgramClassWithMediatr => TemplatePaths.ProgramClassWithMediatrTemplatePath,
            DirectoryContentType.Controller => TemplatePaths.ControllerTemplatePath,
            DirectoryContentType.ControllerWithMediatr => TemplatePaths.ControllerWithMediatrTemplatePath,
            DirectoryContentType.ServiceExtensions => TemplatePaths.ServiceExtensionsTemplatePath,
            DirectoryContentType.ServiceExtensionsWithServices => TemplatePaths.ServiceExtensionsWithServicesTemplatePath,
            DirectoryContentType.GetAllQuery => TemplatePaths.GetAllQueryTemplatePath,
            DirectoryContentType.GetByIdQuery => TemplatePaths.GetByIdQueryTemplatePath,
            DirectoryContentType.CreateCommand => TemplatePaths.CreateCommandTemplatePath,
            DirectoryContentType.UpdateCommand => TemplatePaths.UpdateCommandTemplatePath,
            DirectoryContentType.DeleteCommand => TemplatePaths.DeleteCommandTemplatePath,
            DirectoryContentType.GetAllQueryHandler => TemplatePaths.GetAllQueryHandlerTemplatePath,
            DirectoryContentType.GetByIdQueryHandler => TemplatePaths.GetByIdQueryHandlerTemplatePath,
            DirectoryContentType.CreateCommandHandler => TemplatePaths.CreateCommandHandlerTemplatePath,
            DirectoryContentType.UpdateCommandHandler => TemplatePaths.UpdateCommandHandlerTemplatePath,
            DirectoryContentType.DeleteCommandHandler => TemplatePaths.DeleteCommandHandlerTemplatePath,
            _ => throw new NotImplementedException(),
        };
}
