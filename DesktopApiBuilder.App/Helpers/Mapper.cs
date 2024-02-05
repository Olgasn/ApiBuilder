using DesktopApiBuilder.App.Data.Models.Results;
using DesktopApiBuilder.App.Data.ViewModels;

namespace DesktopApiBuilder.App.Helpers;

public static class Mapper
{
    public static MapResult<Dictionary<string, string>> EntityPropsToDict(List<EntityPropViewModel> entities)
    {
        try
        {
            var dict = new Dictionary<string, string>();

            foreach (var entity in entities)
            {
                dict.Add(entity.Name, entity.Type);
            }

            return new MapResult<Dictionary<string, string>>() { Value = dict };
        }
        catch 
        {
            return new MapResult<Dictionary<string, string>>() { ErrorMessage = "Invalid cast error" };
        }
    }
}
