using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Helpers;

public static class ContentTypeHelper
{
    public static bool DependsOnEntitiesCount(this DirectoryContentType contentType)
    {
        var relatedContentTypes = new DirectoryContentType[] 
        { 
            DirectoryContentType.EntityClass,
            DirectoryContentType.RepositoryClass,
            DirectoryContentType.DtoClass,
            DirectoryContentType.Controller,
            DirectoryContentType.ServiceClass,
            DirectoryContentType.RepositoryInterface,
            DirectoryContentType.ServiceInterface
        };

        return relatedContentTypes.Contains(contentType);
    }
}
