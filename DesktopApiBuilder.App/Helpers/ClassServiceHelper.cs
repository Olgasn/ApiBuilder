using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Data.ViewModels;

namespace DesktopApiBuilder.App.Helpers;

public static class ClassServiceHelper
{
    private const string UseSqlServerMethod = "UseSqlServer";
    private const string UsePostgresMethod = "UseNpgsql";

    public static string GetFileName(EntityClassViewModel? entity, DirectoryContentType contentType) =>
        _ = contentType switch
        {
            DirectoryContentType.EntityClass => entity?.Name ?? string.Empty,
            DirectoryContentType.RepositoryClass => $"{entity?.Name}Repository",
            DirectoryContentType.RepositoryInterface => $"I{entity?.Name}Repository",
            DirectoryContentType.DbContext => "AppDbContext",
            DirectoryContentType.DtoClass => $"{entity?.Name}Dto",
            DirectoryContentType.DtoForCreationClass => $"{entity?.Name}ForCreationDto",
            DirectoryContentType.DtoForUpdateClass => $"{entity?.Name}ForUpdateDto",
            DirectoryContentType.MappingProfile => "MappingProfile",
            DirectoryContentType.ServiceClass => $"{entity?.Name}Service",
            DirectoryContentType.ServiceInterface => $"I{entity?.Name}Service",
            DirectoryContentType.ProgramClass => "Program",
            DirectoryContentType.ProgramClassWithMediatr => "Program",
            DirectoryContentType.Controller => $"{entity?.Name}Controller",
            DirectoryContentType.ControllerWithMediatr => $"{entity?.Name}Controller",
            DirectoryContentType.ServiceExtensions => "ServiceExtensions",
            DirectoryContentType.ServiceExtensionsWithServices => "ServiceExtensions",
            DirectoryContentType.GetAllQuery => $"Get{entity?.PluralName}Query",
            DirectoryContentType.GetByIdQuery => $"Get{entity?.Name}ByIdQuery",
            DirectoryContentType.CreateCommand => $"Create{entity?.Name}Command",
            DirectoryContentType.UpdateCommand => $"Update{entity?.Name}Command",
            DirectoryContentType.DeleteCommand => $"Delete{entity?.Name}Command",
            DirectoryContentType.GetAllQueryHandler => $"Get{entity?.PluralName}QueryHandler",
            DirectoryContentType.GetByIdQueryHandler => $"Get{entity?.Name}ByIdQueryHandler",
            DirectoryContentType.CreateCommandHandler => $"Create{entity?.Name}CommandHandler",
            DirectoryContentType.UpdateCommandHandler => $"Update{entity?.Name}CommandHandler",
            DirectoryContentType.DeleteCommandHandler => $"Delete{entity?.Name}CommandHandler",
            DirectoryContentType.ControllersTests => $"{entity?.Name}ControllerTests",
            DirectoryContentType.ControllersTestsWithMediatr => $"{entity?.Name}ControllerTests",
            DirectoryContentType.ServicesTests => $"{entity?.Name}ServicesTests",
            _ => throw new NotImplementedException()
        };

    public static string GetNamespace(string projectFullName, DirectoryConfig? directory)
    {
        return string.IsNullOrEmpty(directory?.ParentPath)
            ? $"{projectFullName}.{directory?.Name}"
            : $"{projectFullName}.{directory.ParentPath.Trim('/')}.{directory.Name}";
    }

    public static string[] GetUsings(DirectoryContentType contentType, string solutionName, IEnumerable<ProjectConfig>? projects)
    {
        var directories = projects?.SelectMany(p => p.Directories ?? []);

        var entitiesDir = directories.FindByContentType(DirectoryContentType.EntityClass);
        var dtosDir = directories?.FindByContentType(DirectoryContentType.DtoClass, true);
        var repoInterfacesDir = directories?.FindByContentType(DirectoryContentType.RepositoryInterface);
        DirectoryConfig? serviceInterfacesDir;
        DirectoryConfig? repoDir;
        DirectoryConfig? servicesDir;
        DirectoryConfig? queriesDir;
        DirectoryConfig? commandsDir;

        switch (contentType)
        {
            case DirectoryContentType.RepositoryInterface:
                return [GetSpecificUsing(solutionName, projects, entitiesDir)];
            case DirectoryContentType.RepositoryClass:
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                ];
            case DirectoryContentType.ServiceInterface:
                return [GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)];
            case DirectoryContentType.ServiceClass:
                serviceInterfacesDir = directories.FindByContentType(DirectoryContentType.ServiceInterface);
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.Controller:
                serviceInterfacesDir = directories.FindByContentType(DirectoryContentType.ServiceInterface);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.ControllerWithMediatr:
                queriesDir = directories.FindByContentType(DirectoryContentType.GetAllQuery, true);
                commandsDir = directories.FindByContentType(DirectoryContentType.CreateCommand, true);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, queriesDir, true, DirectoryContentType.GetAllQuery),
                    GetSpecificUsing(solutionName, projects, commandsDir, true, DirectoryContentType.CreateCommand)
                ];
            case DirectoryContentType.DbContext:
                return [GetSpecificUsing(solutionName, projects, entitiesDir)];
            case DirectoryContentType.MappingProfile:
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)
                ];
            case DirectoryContentType.ServiceExtensions:
                repoDir = directories.FindByContentType(DirectoryContentType.RepositoryClass);
                return [
                    GetGeneralProjectUsing(solutionName, projects, DirectoryContentType.RepositoryClass),
                    GetSpecificUsing(solutionName, projects, repoDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir)
                ];
            case DirectoryContentType.ServiceExtensionsWithServices:
                repoDir = directories.FindByContentType(DirectoryContentType.RepositoryClass);
                serviceInterfacesDir = directories.FindByContentType(DirectoryContentType.ServiceInterface);
                servicesDir = directories.FindByContentType(DirectoryContentType.ServiceClass);
                return [
                    GetGeneralProjectUsing(solutionName, projects, EnumHelper.GetContentTypeFromString(entitiesDir?.ContentType)),
                    GetSpecificUsing(solutionName, projects, repoDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, servicesDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.ProgramClass:
                var extensionsWithServicesDir = directories.FindByContentType(DirectoryContentType.ServiceExtensionsWithServices);
                return [
                    GetSpecificUsing(solutionName, projects, extensionsWithServicesDir),
                    GetGeneralProjectUsing(solutionName, projects, DirectoryContentType.DtoClass, true)
                ];
            case DirectoryContentType.ProgramClassWithMediatr:
                var extensionsDir = directories.FindByContentType(DirectoryContentType.ServiceExtensions);
                queriesDir = directories.FindByContentType(DirectoryContentType.GetAllQuery, true);
                return [
                    GetGeneralProjectUsing(solutionName, projects, DirectoryContentType.DtoClass, true),
                    GetSpecificUsing(solutionName, projects, extensionsDir),
                    GetSpecificUsing(solutionName, projects, queriesDir, true, DirectoryContentType.GetAllQuery)
                ];
            case DirectoryContentType.GetAllQuery:
                return [GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)];
            case DirectoryContentType.GetByIdQuery:
                return [GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)];
            case DirectoryContentType.CreateCommand:
                return [GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)];
            case DirectoryContentType.UpdateCommand:
                return [GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass)];
            case DirectoryContentType.GetAllQueryHandler:
                var getAllQueryDir = directories.FindByContentType(DirectoryContentType.GetAllQuery, true);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, getAllQueryDir, true, DirectoryContentType.GetAllQuery)
                ];
            case DirectoryContentType.GetByIdQueryHandler:
                var getByIdQueryDir = directories.FindByContentType(DirectoryContentType.GetByIdQuery, true);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, getByIdQueryDir, true, DirectoryContentType.GetByIdQuery)
                ];
            case DirectoryContentType.CreateCommandHandler:
                var createCommandDir = directories.FindByContentType(DirectoryContentType.CreateCommand, true);
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, createCommandDir, true, DirectoryContentType.CreateCommand)
                ];
            case DirectoryContentType.UpdateCommandHandler:
                var updateCommandDir = directories.FindByContentType(DirectoryContentType.UpdateCommand, true);
                return [
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, updateCommandDir, true, DirectoryContentType.UpdateCommand)
                ];
            case DirectoryContentType.DeleteCommandHandler:
                var deleteCommandDir = directories.FindByContentType(DirectoryContentType.DeleteCommand, true);
                return [
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, deleteCommandDir, true, DirectoryContentType.DeleteCommand)
                ];
            case DirectoryContentType.ControllersTests:
                var controllersDir = directories.FindByContentType(DirectoryContentType.Controller);
                serviceInterfacesDir = directories.FindByContentType(DirectoryContentType.ServiceInterface);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir),
                    GetSpecificUsing(solutionName, projects, controllersDir)
                ];
            case DirectoryContentType.ControllersTestsWithMediatr:
                var controllersWithMediatrDir = directories.FindByContentType(DirectoryContentType.ControllerWithMediatr);
                queriesDir = directories.FindByContentType(DirectoryContentType.GetAllQuery, true);
                commandsDir = directories.FindByContentType(DirectoryContentType.CreateCommand, true);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, queriesDir, true, DirectoryContentType.GetAllQuery),
                    GetSpecificUsing(solutionName, projects, commandsDir, true, DirectoryContentType.CreateCommand),
                    GetSpecificUsing(solutionName, projects, controllersWithMediatrDir)
                ];
            case DirectoryContentType.ServicesTests:
                servicesDir = directories.FindByContentType(DirectoryContentType.ServiceClass);
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, servicesDir),
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir)
                ];
        }

        return [];
    }

    public static string GetSpecificUsing(string solutionName, IEnumerable<ProjectConfig>? projects, 
        DirectoryConfig? directory, bool multipleContentTypes = false, DirectoryContentType? contentTypeToCheckIfMultiple = null) =>
        multipleContentTypes
        ? GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(contentTypeToCheckIfMultiple.ToString())))?.Name}", directory)
        : GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == directory?.ContentType))?.Name}", directory);

    public static string GetGeneralProjectUsing(string solutionName, IEnumerable<ProjectConfig>? projects, DirectoryContentType contentType, bool multipleContentTypes = false) =>
        multipleContentTypes
        ? $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(contentType.ToString())))?.Name}"
        : $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == contentType.ToString()))?.Name}";

    public static string GetUseSqlProviderMethodName(SqlProviders sqlProvider) =>
        _ = sqlProvider switch
        {
            SqlProviders.MSSqlServer => UseSqlServerMethod,
            SqlProviders.Postgres => UsePostgresMethod,
            _ => throw new NotImplementedException()
        };
}
