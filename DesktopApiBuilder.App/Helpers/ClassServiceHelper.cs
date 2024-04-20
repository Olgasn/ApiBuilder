using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Configs;
using DesktopApiBuilder.App.Data.ViewModels;

namespace DesktopApiBuilder.App.Helpers;

public static class ClassServiceHelper
{
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
            _ => throw new NotImplementedException(),
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
        var repoInterfacesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.RepositoryInterface.ToString());
        DirectoryConfig? serviceInterfacesDir;
        DirectoryConfig? repoDir;
        DirectoryConfig? queriesDir;
        // TODO: replace with extension methods
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
                queriesDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.GetAllQuery.ToString()));
                var commandsDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.CreateCommand.ToString()));
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
                    // TODO: use root content types for checking
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == DirectoryContentType.RepositoryClass.ToString()))?.Name}",
                    GetSpecificUsing(solutionName, projects, repoDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir)
                ];
            case DirectoryContentType.ServiceExtensionsWithServices:
                repoDir = directories.FindByContentType(DirectoryContentType.RepositoryClass);
                serviceInterfacesDir = directories.FindByContentType(DirectoryContentType.ServiceInterface);
                var serviceDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceClass.ToString());
                return [
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}",
                    GetSpecificUsing(solutionName, projects, repoDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, serviceDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.ProgramClass:
                // TODO: services extensions check
                var extensionsWithServicesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceExtensionsWithServices.ToString());
                return [
                    GetSpecificUsing(solutionName, projects, extensionsWithServicesDir),
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.DtoClass.ToString())))?.Name}"
                ];
            case DirectoryContentType.ProgramClassWithMediatr:
                var extensionsDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceExtensions.ToString());
                queriesDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.GetAllQuery.ToString()));
                return [
                    // TODO: add method for it
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.DtoClass.ToString())))?.Name}",
                    GetSpecificUsing(solutionName, projects, extensionsDir),
                    GetSpecificUsing(solutionName, projects, queriesDir, true, DirectoryContentType.GetAllQuery),
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
                var getAllQueryDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.GetAllQuery.ToString()));
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, getAllQueryDir, true, DirectoryContentType.GetAllQuery)
                ];
            case DirectoryContentType.GetByIdQueryHandler:
                var getByIdQueryDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.GetByIdQuery.ToString()));
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir, true, DirectoryContentType.DtoClass),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, getByIdQueryDir, true, DirectoryContentType.GetByIdQuery)
                ];
            case DirectoryContentType.CreateCommandHandler:
                var createCommandDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.CreateCommand.ToString()));
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, createCommandDir, true, DirectoryContentType.CreateCommand)
                ];
            case DirectoryContentType.UpdateCommandHandler:
                var updateCommandDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.UpdateCommand.ToString()));
                return [
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, updateCommandDir, true, DirectoryContentType.UpdateCommand)
                ];
            case DirectoryContentType.DeleteCommandHandler:
                var deleteCommandDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.DeleteCommand.ToString()));
                return [
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, deleteCommandDir, true, DirectoryContentType.DeleteCommand)
                ];
        }

        return [];
    }

    public static string GetSpecificUsing(string solutionName, IEnumerable<ProjectConfig>? projects, 
        DirectoryConfig? directory, bool multipleContentTypes = false, DirectoryContentType? contentTypeToCheckIfMultiple = null) =>
        multipleContentTypes
        ? GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(contentTypeToCheckIfMultiple.ToString())))?.Name}", directory)
        : GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == directory?.ContentType))?.Name}", directory);
}
