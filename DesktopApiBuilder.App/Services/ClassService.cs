using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Data.ViewModels;
using DesktopApiBuilder.App.Helpers;
using System.Text;

namespace DesktopApiBuilder.App.Services;

public static class ClassService
{
    private const string Path = "C:\\D\\Projects\\test";

    private const string EntityPropTemplate = "\r\n\tpublic {0} {1} {{ get; set; }}";
    private const string DbSetTemplate = "\r\n\tpublic DbSet<{0}> {1} {{ get; set; }}";

    private const string MappingItemTemplate = "\r\n\t\tCreateMap<{0}, {0}Dto>();";

    private const string RepositoryRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Repository, {0}Repository>();";
    private const string ServiceRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Service, {0}Service>();";

    private const string ControllerTemplatePath = "wwwroot\\templates\\api\\ControllerTemplate.txt";
    
    private const string ProgramClassTemplatePath = "wwwroot\\templates\\api\\ApiProgramClassTemplate.txt";
    
    private const string ServiceExtensionsTemplatePath = "wwwroot\\templates\\api\\ServiceExtensionsTemplate.txt";

    public static void CreateClasses(SolutionSettingsModel solutionSettings, List<EntityClassViewModel> entities)
    {
        SolutionConfig? config = ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);

        try
        {
            foreach (var project in config?.Projects ?? [])
            {
                var projectPath = ConfigHelper.GetProjectPath(config, project, solutionSettings.SolutionName);

                foreach (var directory in project.Directories ?? [])
                {
                    if (string.IsNullOrEmpty(directory.ContentType) 
                        || !Enum.TryParse(typeof(DirectoryContentType), directory.ContentType, out object? contentTypeObj))
                    {
                        continue;
                    }

                    var contentType = (DirectoryContentType)contentTypeObj;
                    var fileContent = TemplateHelper.GetTemplateContent(contentType);
                    var classNamespace = GetClassNamespace($"{solutionSettings.SolutionName}.{project.Name}", directory);

                    foreach (var entity in entities)
                    {
                        var className = GetClassName(entity.Name, contentType);
                        var filePath = $"{solutionSettings.SolutionPath}/{projectPath}/{solutionSettings.SolutionName}.{project.Name}{directory.ParentPath}/{directory.Name}/{className}.cs";

                        var classSettings = new ClassTemplateSettings()
                        {
                            Entity = entity,
                            ContentType = contentType,
                            Template = fileContent,
                            ClassName = className,
                            Namespace = classNamespace,
                            Usings = GetUsings(contentType, solutionSettings.SolutionName, 
                                $"{solutionSettings.SolutionName}.{project.Name}", config?.Projects)
                        };

                        StreamWriter sw = new(filePath);
                        sw.WriteLine(GetFormattedString(classSettings));

                        sw.Close();
                    }
                }

                foreach (var rootContentType in project.RootContentTypes ?? [])
                {
                    if (string.IsNullOrEmpty(rootContentType)
                        || !Enum.TryParse(typeof(DirectoryContentType), rootContentType, out object? contentTypeObj))
                    {
                        continue;
                    }

                    var contentType = (DirectoryContentType)contentTypeObj;
                    var fileContent = TemplateHelper.GetTemplateContent(contentType);

                    var className = GetClassName(string.Empty, contentType);
                    var filePath = $"{solutionSettings.SolutionPath}/{projectPath}/{solutionSettings.SolutionName}.{project.Name}/{className}.cs";

                    var classSettings = new ClassTemplateSettings()
                    {
                        Entities = entities,
                        ContentType = contentType,
                        Template = fileContent,
                        ClassName = className,
                        Namespace = $"{solutionSettings.SolutionName}.{project.Name}",
                        Usings = GetUsings(contentType, solutionSettings.SolutionName, 
                            $"{solutionSettings.SolutionName}.{project.Name}", config?.Projects)
                    };

                    StreamWriter sw = new(filePath);
                    sw.WriteLine(GetFormattedString(classSettings));

                    sw.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static string GetFormattedString(ClassTemplateSettings classSettings)
    {
        StringBuilder? props;

        switch (classSettings.ContentType)
        {
            case DirectoryContentType.EntityClass:
                return GetEntityCreationTemplate(classSettings);
            case DirectoryContentType.DtoClass:
                return GetEntityCreationTemplate(classSettings);
            case DirectoryContentType.RepositoryInterface:
                return string.Format(
                    classSettings.Template ?? string.Empty, 
                    classSettings.Usings[0], 
                    classSettings.Namespace, 
                    classSettings.Entity?.Name, 
                    "int");
            case DirectoryContentType.RepositoryClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    classSettings.Entity?.PluralName,
                    "int");
            case DirectoryContentType.ServiceInterface:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    "int");
            case DirectoryContentType.ServiceClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Usings[3],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    "int");
            case DirectoryContentType.DbContext:
                props = new StringBuilder();
                foreach (var entity in classSettings.Entities ?? [])
                {
                    props.Append(string.Format(DbSetTemplate, entity.Name, entity.PluralName));
                }
                return string.Format(classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    props);
            case DirectoryContentType.MappingProfile:
                props = new StringBuilder();
                foreach (var entity in classSettings.Entities ?? [])
                {
                    props.Append(string.Format(MappingItemTemplate, entity.Name));
                }
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    props);
        }

        return string.Empty;
    }

    private static string GetClassName(string entityName, DirectoryContentType contentType) =>
        _ = contentType switch
        {
            DirectoryContentType.EntityClass => entityName,
            DirectoryContentType.RepositoryClass => $"{entityName}Repository",
            DirectoryContentType.RepositoryInterface => $"I{entityName}Repository",
            DirectoryContentType.DbContext => "AppDbContext",
            DirectoryContentType.DtoClass => $"{entityName}Dto",
            DirectoryContentType.MappingProfile => "MappingProfile",
            DirectoryContentType.ServiceClass => $"{entityName}Service",
            DirectoryContentType.ServiceInterface => $"I{entityName}Service",
            DirectoryContentType.ProgramClass => throw new NotImplementedException(),
            DirectoryContentType.Controller => throw new NotImplementedException(),
            DirectoryContentType.ServiceExtensionsClass => throw new NotImplementedException(),
            _ => throw new NotImplementedException(),
        };

    private static string GetClassNamespace(string projectFullName, DirectoryConfig? directory)
    {
        return string.IsNullOrEmpty(directory?.ParentPath) 
            ? $"{projectFullName}.{directory?.Name}" 
            : $"{projectFullName}.{directory.ParentPath.Trim('/')}.{directory.Name}";
    }

    private static string[] GetUsings(DirectoryContentType contentType, string solutionName,
        string projectFullName, IEnumerable<ProjectConfig>? projects)
    {
        var directories = projects?.SelectMany(p => p.Directories ?? []);

        var entitiesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.EntityClass.ToString());
        var dtosDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.DtoClass.ToString());
        var repoInterfacesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.RepositoryInterface.ToString());
        var serviceInterfacesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceInterface.ToString());

        switch (contentType)
        {
            case DirectoryContentType.RepositoryInterface:
                return [GetClassNamespace(projectFullName, entitiesDir)];
            case DirectoryContentType.RepositoryClass:
                return [
                    GetClassNamespace(projectFullName, entitiesDir),
                    GetClassNamespace(projectFullName, repoInterfacesDir),
                ];
            case DirectoryContentType.ServiceInterface:
                return [GetClassNamespace(projectFullName, dtosDir)];
            case DirectoryContentType.ServiceClass:
                return [
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}", entitiesDir),
                    GetClassNamespace(projectFullName, dtosDir),
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == repoInterfacesDir?.ContentType))?.Name}", repoInterfacesDir),
                    GetClassNamespace(projectFullName, serviceInterfacesDir)
                ];
            case DirectoryContentType.DbContext:
                return [GetClassNamespace(projectFullName, entitiesDir)];
            case DirectoryContentType.MappingProfile:
                return [
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}", entitiesDir),
                    GetClassNamespace(projectFullName, dtosDir)
                ];
        }

        return [];
    }

    private static string GetEntityCreationTemplate(ClassTemplateSettings classSettings)
    {
        var props = new StringBuilder();
        foreach (var prop in classSettings.Entity?.Properties ?? [])
        {
            props.Append(string.Format(EntityPropTemplate, prop.Type, prop.Name));
        }

        return string.Format(classSettings.Template ?? string.Empty, classSettings.Namespace, classSettings.Entity?.Name, props);
    }

    public static void CreateController(string solutionName, EntityClassViewModel entity)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ControllerTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Controllers\\{entity.Name}Controller.cs");

            sw.WriteLine(string.Format(fileContent, 
                $"{solutionName}.BLL.Dtos", 
                $"{solutionName}.BLL.Services.Abstractions",
                $"{solutionName}.API.Controllers",
                $"{entity.PluralName[..1].ToLower()}{entity.PluralName[1..]}",
                entity.Name,
                $"{entity.Name[..1].ToLower()}{entity.Name[1..]}",
                "int"));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void CreateServiceExtensions(string solutionName, List<EntityClassViewModel> entities)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ServiceExtensionsTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Extensions\\ServiceExtensions.cs");

            var repositoryProps = string.Empty;
            var serviceProps = string.Empty;
            foreach (var entity in entities)
            {
                repositoryProps += string.Format(RepositoryRegTemplate, entity.Name);
                serviceProps += string.Format(ServiceRegTemplate, entity.Name);
            }

            sw.WriteLine(string.Format(fileContent,
                $"{solutionName}.DAL",
                $"{solutionName}.DAL.Repositories",
                $"{solutionName}.DAL.Repositories.Abstractions",
                $"{solutionName}.BLL.Services",
                $"{solutionName}.BLL.Services.Abstractions",
                $"{solutionName}.API.Extensions",
                repositoryProps,
                serviceProps));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException);
        }
    }

    public static void UpdateProgramClass(string solutionName)
    {
        try
        {
            var fileContent = TemplateHelper.GetTemplateContent(ProgramClassTemplatePath);

            StreamWriter sw = new($"{Path}\\{solutionName}\\{solutionName}.API\\Program.cs");

            sw.WriteLine(string.Format(fileContent, 
                $"{solutionName}.API.Extensions",
                $"{solutionName}.BLL"));

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
