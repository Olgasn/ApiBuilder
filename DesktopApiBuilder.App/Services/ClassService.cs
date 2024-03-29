using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Data.ViewModels;
using DesktopApiBuilder.App.Helpers;
using System.Text;

namespace DesktopApiBuilder.App.Services;

public static class ClassService
{
    private const string EntityPropTemplate = "\r\n\tpublic {0} {1} {{ get; set; }}";
    private const string DbSetTemplate = "\r\n\tpublic DbSet<{0}> {1} {{ get; set; }}";

    private const string MappingItemTemplate = "\r\n\t\tCreateMap<{0}, {0}Dto>().ReverseMap();";

    private const string RepositoryRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Repository, {0}Repository>();";
    private const string ServiceRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Service, {0}Service>();";

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
                            Entities = contentType == DirectoryContentType.ServiceExtensions ? entities : null,
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
        var props = new StringBuilder();

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
            case DirectoryContentType.Controller:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    $"{classSettings.Entity?.PluralName[..1].ToLower()}{classSettings.Entity?.PluralName[1..]}", 
                    classSettings.Entity?.Name,
                    $"{classSettings.Entity?.Name[..1].ToLower()}{classSettings.Entity?.Name[1..]}",
                    "int");
            case DirectoryContentType.DbContext:
                foreach (var entity in classSettings.Entities ?? [])
                {
                    props.Append(string.Format(DbSetTemplate, entity.Name, entity.PluralName));
                }
                return string.Format(classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    props);
            case DirectoryContentType.MappingProfile:
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
            case DirectoryContentType.ServiceExtensions:
                var repositoryProps = new StringBuilder();
                var serviceProps = new StringBuilder();
                foreach (var entityName in (classSettings.Entities ?? []).Select(e => e.Name))
                {
                    repositoryProps.Append(string.Format(RepositoryRegTemplate, entityName));
                    serviceProps.Append(string.Format(ServiceRegTemplate, entityName));
                }
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Usings[3],
                    classSettings.Usings[4],
                    classSettings.Namespace,
                    repositoryProps,
                    serviceProps);
            case DirectoryContentType.ProgramClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1]);
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
            DirectoryContentType.ProgramClass => "Program",
            DirectoryContentType.Controller => $"{entityName}Controller",
            DirectoryContentType.ServiceExtensions => "ServiceExtensions",
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
            case DirectoryContentType.Controller:
                return [
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == dtosDir?.ContentType))?.Name}", dtosDir),
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == serviceInterfacesDir?.ContentType))?.Name}", serviceInterfacesDir)
                ];
            case DirectoryContentType.DbContext:
                return [GetClassNamespace(projectFullName, entitiesDir)];
            case DirectoryContentType.MappingProfile:
                return [
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}", entitiesDir),
                    GetClassNamespace(projectFullName, dtosDir)
                ];
            case DirectoryContentType.ServiceExtensions:
                var repoDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.RepositoryClass.ToString());
                var serviceDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceClass.ToString());
                return [
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}",
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == repoDir?.ContentType))?.Name}", repoDir),
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == repoInterfacesDir?.ContentType))?.Name}", repoInterfacesDir),
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == serviceDir?.ContentType))?.Name}", serviceDir),
                    GetClassNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == serviceInterfacesDir?.ContentType))?.Name}", serviceInterfacesDir)
                ];
            case DirectoryContentType.ProgramClass:
                var extensionsDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceExtensions.ToString());
                return [
                    GetClassNamespace(projectFullName, extensionsDir),
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == dtosDir?.ContentType))?.Name}"
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
}
