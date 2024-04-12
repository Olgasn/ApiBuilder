using DesktopApiBuilder.App.Data.Enums;

namespace DesktopApiBuilder.App.Helpers;

public static class EnumHelper
{
    public static string GetIdTypeName(IdType idType) =>
        _ = idType switch
        {
            IdType.Int => "int",
            IdType.Guid => "Guid",
            _ => throw new NotImplementedException(),
        };

    public static DirectoryContentType GetContentTypeFromString(string? contentTypeItem)
    {
        if (string.IsNullOrEmpty(contentTypeItem)
            || !Enum.TryParse(typeof(DirectoryContentType), contentTypeItem, out object? contentTypeObj))
        {
            return DirectoryContentType.Undefined;
        }

        return  (DirectoryContentType)contentTypeObj;
    }
}
