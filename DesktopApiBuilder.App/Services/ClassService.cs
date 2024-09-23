using DesktopApiBuilder.App.Data.Constants;
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
    private const string IncludeNavPropTemplate = ".Include(e => e.{0})";

    private const string MappingItemTemplate = "\r\n\t\tCreateMap<{0}, {1}>();";

    private const string RepositoryRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Repository, {0}Repository>();";
    private const string ServiceRegTemplate = "\r\n\t\tservices.AddScoped<I{0}Service, {0}Service>();";

    public static void CreateClasses(SolutionSettingsModel solutionSettings, List<EntityClassViewModel> entities)
    {
        var config = solutionSettings.ArchitectureType == ArchitectureType.Custom
            ? solutionSettings.CustomSolutionConfig
            : ConfigHelper.GetSolutionConfig(solutionSettings.ArchitectureType);

        try
        {
            foreach (var project in config?.Projects ?? [])
            {
                var projectPath = ConfigHelper.GetProjectPath(config, project, solutionSettings.SolutionName);

                foreach (var directory in project.Directories ?? [])
                {
                    foreach (var contentTypeListItem in directory.ContentTypeList ?? [])
                    {
                        var contentTypeItem = EnumHelper.GetContentTypeFromString(contentTypeListItem);
                        if (contentTypeItem == DirectoryContentType.Undefined) continue;

                        CreateClassesRelatedToEntities(solutionSettings, entities, contentTypeItem, config, project, directory);
                    }

                    var contentType = EnumHelper.GetContentTypeFromString(directory.ContentType);
                    if (contentType == DirectoryContentType.Undefined) continue;

                    CreateClassesRelatedToEntities(solutionSettings, entities, contentType, config, project, directory);
                }

                foreach (var rootContentType in project.RootContentTypes ?? [])
                {
                    var contentType = EnumHelper.GetContentTypeFromString(rootContentType);
                    if (contentType == DirectoryContentType.Undefined) continue;

                    var fileContent = TemplateHelper.GetTemplateContent(contentType);

                    var className = ClassServiceHelper.GetFileName(null, contentType);
                    var filePath = $"{solutionSettings.SolutionPath}/{projectPath}/{solutionSettings.SolutionName}.{project.Name}/{className}.cs";

                    var classSettings = new ClassTemplateSettings()
                    {
                        Entities = entities,
                        ContentType = contentType,
                        Template = fileContent,
                        ClassName = className,
                        Namespace = $"{solutionSettings.SolutionName}.{project.Name}",
                        Usings = ClassServiceHelper.GetUsings(contentType, solutionSettings.SolutionName, config?.Projects),
                        IdType = solutionSettings.IdType
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
        var lowercaseEntityName = $"{classSettings.Entity?.Name?[..1].ToLower()}{classSettings.Entity?.Name?[1..]}";
        var lowercaseEntityPluralName = $"{classSettings.Entity?.PluralName?[..1].ToLower()}{classSettings.Entity?.PluralName?[1..]}";

        string entityIdTestField;

        switch (classSettings.ContentType)
        {
            case DirectoryContentType.EntityClass:
                return GetEntityCreationTemplate(classSettings);
            case DirectoryContentType.DtoClass:
                return GetDtoCreationTemplate(classSettings, includeNavProps: true);
            case DirectoryContentType.DtoForCreationClass:
                return GetDtoCreationTemplate(classSettings, includeId: false);
            case DirectoryContentType.DtoForUpdateClass:
                return GetDtoCreationTemplate(classSettings);
            case DirectoryContentType.RepositoryInterface:
                return string.Format(
                    classSettings.Template ?? string.Empty, 
                    classSettings.Usings[0], 
                    classSettings.Namespace, 
                    classSettings.Entity?.Name, 
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.RepositoryClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    classSettings.Entity?.PluralName,
                    GetIncludesChain(classSettings.Entity?.Properties ?? []),
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.ServiceInterface:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.ServiceClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Usings[3],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.Controller:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    lowercaseEntityPluralName, 
                    classSettings.Entity?.Name,
                    lowercaseEntityName,
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.ControllerWithMediatr:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Namespace,
                    lowercaseEntityPluralName,
                    classSettings.Entity?.Name,
                    classSettings.Entity?.PluralName,
                    EnumHelper.GetIdTypeName(classSettings.IdType),
                    lowercaseEntityName);
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
                var index = 0;
                foreach (var entityName in (classSettings.Entities ?? []).Select(e => e.Name))
                {
                    props.Append(string.Format(MappingItemTemplate, entityName, $"{entityName}Dto"));
                    props.Append(string.Format(MappingItemTemplate, $"{entityName}ForCreationDto", entityName));
                    props.Append(string.Format(MappingItemTemplate, $"{entityName}ForUpdateDto", entityName));
                    if (index != classSettings.Entities?.Count - 1)
                    {
                        props.Append('\n');
                    }
                    
                    index++;
                }
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    props);
            case DirectoryContentType.ServiceExtensions:
                props = new StringBuilder();
                foreach (var entityName in (classSettings.Entities ?? []).Select(e => e.Name))
                {
                    props.Append(string.Format(RepositoryRegTemplate, entityName));
                }
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Namespace,
                    classSettings.UseSqlProviderMethodName,
                    props);
            case DirectoryContentType.ServiceExtensionsWithServices:
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
                    classSettings.UseSqlProviderMethodName,
                    repositoryProps,
                    serviceProps);
            case DirectoryContentType.ProgramClass:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1]);
            case DirectoryContentType.ProgramClassWithMediatr:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Entities?[0].PluralName);
            case DirectoryContentType.GetAllQuery:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.PluralName,
                    classSettings.Entity?.Name);
            case DirectoryContentType.GetByIdQuery:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.CreateCommand:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.UpdateCommand:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.DeleteCommand:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Namespace,
                    classSettings.Entity?.Name,
                    EnumHelper.GetIdTypeName(classSettings.IdType));
            case DirectoryContentType.GetAllQueryHandler:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Namespace,
                    classSettings.Entity?.PluralName,
                    classSettings.Entity?.Name);
            case DirectoryContentType.GetByIdQueryHandler:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.CreateCommandHandler:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.UpdateCommandHandler:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.DeleteCommandHandler:
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Namespace,
                    classSettings.Entity?.Name);
            case DirectoryContentType.ControllersTests:
                entityIdTestField = classSettings.IdType is IdType.Int
                    ? $"const int {lowercaseEntityName}Id = 1;"
                    : $"var {lowercaseEntityName}Id = Guid.NewGuid();";
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2], 
                    classSettings.Namespace,
                    classSettings.Entity.Name,
                    classSettings.Entity.PluralName,
                    lowercaseEntityPluralName,
                    entityIdTestField,
                    lowercaseEntityName);
            case DirectoryContentType.ControllersTestsWithMediatr:
                entityIdTestField = classSettings.IdType is IdType.Int
                    ? $"const int {lowercaseEntityName}Id = 1;"
                    : $"var {lowercaseEntityName}Id = Guid.NewGuid();";
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Usings[3],
                    classSettings.Namespace,
                    classSettings.Entity.Name,
                    classSettings.Entity.PluralName,
                    lowercaseEntityPluralName,
                    entityIdTestField,
                    lowercaseEntityName);
            case DirectoryContentType.ServicesTests:
                entityIdTestField = classSettings.IdType is IdType.Int
                    ? $"const int {lowercaseEntityName}Id = 1;"
                    : $"var {lowercaseEntityName}Id = Guid.NewGuid();";
                return string.Format(
                    classSettings.Template ?? string.Empty,
                    classSettings.Usings[0],
                    classSettings.Usings[1],
                    classSettings.Usings[2],
                    classSettings.Usings[3],
                    classSettings.Namespace,
                    classSettings.Entity.Name,
                    classSettings.Entity.PluralName,
                    lowercaseEntityPluralName,
                    entityIdTestField,
                    lowercaseEntityName);
        }

        return string.Empty;
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

    private static string GetDtoCreationTemplate(ClassTemplateSettings classSettings, bool includeId = true, bool includeNavProps = false)
    {
        var props = new StringBuilder();
        foreach (var prop in (classSettings.Entity?.Properties ?? []).Where(prop => (includeId || prop.Name != "Id") && (includeNavProps || DefaultEntityPropsTypes.Types.Contains(prop.Type))))
        {
            if (!DefaultEntityPropsTypes.Types.Contains(prop.Type))
            {
                props.Append(string.Format(EntityPropTemplate, $"{prop.Type}Dto", prop.Name));
                continue;
            }

            props.Append(string.Format(EntityPropTemplate, prop.Type, prop.Name));
        }

        return string.Format(classSettings.Template ?? string.Empty, classSettings.Namespace, classSettings.Entity?.Name, props);
    }

    private static string GetIncludesChain(List<EntityPropViewModel> props)
    {
        var result = new StringBuilder();
        foreach (var prop in props.Where(p => !DefaultEntityPropsTypes.Types.Contains(p.Type)))
        {
            result.Append(string.Format(IncludeNavPropTemplate, prop.Name));
        }

        return result.ToString();
    }

    private static void CreateClassesRelatedToEntities(SolutionSettingsModel solutionSettings, List<EntityClassViewModel> entities, 
        DirectoryContentType contentType, SolutionConfig? config, ProjectConfig project, DirectoryConfig directory)
    {
        var projectPath = ConfigHelper.GetProjectPath(config, project, solutionSettings.SolutionName);
        var fileContent = TemplateHelper.GetTemplateContent(contentType);
        var classNamespace = ClassServiceHelper.GetNamespace($"{solutionSettings.SolutionName}.{project.Name}", directory);

        foreach (var entity in entities)
        {
            var className = ClassServiceHelper.GetFileName(entity, contentType);
            var filePath = $"{solutionSettings.SolutionPath}/{projectPath}/{solutionSettings.SolutionName}.{project.Name}{directory.ParentPath}/{directory.Name}/{className}.cs";

            var classSettings = new ClassTemplateSettings()
            {
                Entity = entity,
                Entities = entities,
                ContentType = contentType,
                Template = fileContent,
                ClassName = className,
                Namespace = classNamespace,
                Usings = ClassServiceHelper.GetUsings(contentType, solutionSettings.SolutionName, config?.Projects),
                IdType = solutionSettings.IdType,
                UseSqlProviderMethodName = ClassServiceHelper.GetUseSqlProviderMethodName(solutionSettings.SqlProvider)
            };

            StreamWriter sw = new(filePath);
            sw.WriteLine(GetFormattedString(classSettings));

            sw.Close();
        }
    }
}
