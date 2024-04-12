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
            DirectoryContentType.MappingProfile => "MappingProfile",
            DirectoryContentType.ServiceClass => $"{entity?.Name}Service",
            DirectoryContentType.ServiceInterface => $"I{entity?.Name}Service",
            DirectoryContentType.ProgramClass => "Program",
            DirectoryContentType.Controller => $"{entity?.Name}Controller",
            DirectoryContentType.ServiceExtensions => "ServiceExtensions",
            DirectoryContentType.GetAllQuery => $"GetAll{entity?.PluralName}Query",
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

        var entitiesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.EntityClass.ToString());
        var dtosDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.DtoClass.ToString());
        var repoInterfacesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.RepositoryInterface.ToString());
        var serviceInterfacesDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceInterface.ToString());

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
                return [GetSpecificUsing(solutionName, projects, dtosDir)];
            case DirectoryContentType.ServiceClass:
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, dtosDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.Controller:
                return [
                    GetSpecificUsing(solutionName, projects, dtosDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.DbContext:
                return [GetSpecificUsing(solutionName, projects, entitiesDir)];
            case DirectoryContentType.MappingProfile:
                return [
                    GetSpecificUsing(solutionName, projects, entitiesDir),
                    GetSpecificUsing(solutionName, projects, dtosDir)
                ];
            case DirectoryContentType.ServiceExtensions:
                var repoDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.RepositoryClass.ToString());
                var serviceDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceClass.ToString());
                return [
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == entitiesDir?.ContentType))?.Name}",
                    GetSpecificUsing(solutionName, projects, repoDir),
                    GetSpecificUsing(solutionName, projects, repoInterfacesDir),
                    GetSpecificUsing(solutionName, projects, serviceDir),
                    GetSpecificUsing(solutionName, projects, serviceInterfacesDir)
                ];
            case DirectoryContentType.ProgramClass:
                var extensionsDir = directories?.FirstOrDefault(d => d.ContentType == DirectoryContentType.ServiceExtensions.ToString());
                return [
                    GetSpecificUsing(solutionName, projects, extensionsDir),
                    $"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == dtosDir?.ContentType))?.Name}"
                ];
            case DirectoryContentType.GetAllQuery:
                return [GetSpecificUsing(solutionName, projects, entitiesDir)];
                // example
                //var queriesDir = directories?.FirstOrDefault(d => (d.ContentTypeList ?? []).Contains(DirectoryContentType.GetAllQuery.ToString()));
                //return [GetSpecificUsing(solutionName, projects, queriesDir, true, DirectoryContentType.GetAllQuery)];
        }

        return [];
    }

    public static string GetSpecificUsing(string solutionName, IEnumerable<ProjectConfig>? projects, 
        DirectoryConfig? directory, bool multipleContentTypes = false, DirectoryContentType? contentTypeToCheckIfMultiple = null) =>
        multipleContentTypes
        ? GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => (d.ContentTypeList ?? []).Contains(contentTypeToCheckIfMultiple.ToString())))?.Name}", directory)
        : GetNamespace($"{solutionName}.{projects?.FirstOrDefault(p => (p.Directories ?? []).Any(d => d.ContentType == directory?.ContentType))?.Name}", directory);
}
