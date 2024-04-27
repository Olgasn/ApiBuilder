using DesktopApiBuilder.App.Data.Constants;
using DesktopApiBuilder.App.Data.Enums;
using DesktopApiBuilder.App.Data.Models.Results;
using DesktopApiBuilder.App.Data.ViewModels;

namespace DesktopApiBuilder.App.Helpers;

public static class Mapper
{
    public static MapResult<Dictionary<string, string>> EntityPropsToDict(List<EntityPropViewModel> properties, IdType? idType = null)
    {
        try
        {
            var dict = new Dictionary<string, string>();

            foreach (var prop in properties)
            {
                if (string.IsNullOrWhiteSpace(prop.Name) || string.IsNullOrWhiteSpace(prop.Type))
                    continue;

                if (!DefaultEntityPropsTypes.Types.Contains(prop.Type) && !properties.Exists(p => p.Name == $"{prop.Name}Id"))
                {
                    dict.Add($"{prop.Name}Id", EnumHelper.GetIdTypeName(idType));
                }

                dict.Add(prop.Name, prop.Type);
            }

            return new MapResult<Dictionary<string, string>>() { Value = dict };
        }
        catch (Exception ex) 
        {
            return new MapResult<Dictionary<string, string>>() { ErrorMessage = ex.Message };
        }
    }
}
