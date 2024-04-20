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
                if (string.IsNullOrWhiteSpace(entity.Name) || string.IsNullOrWhiteSpace(entity.Type))
                    continue;

                dict.Add(entity.Name, entity.Type);
            }

            return new MapResult<Dictionary<string, string>>() { Value = dict };
        }
        catch (Exception ex) 
        {
            return new MapResult<Dictionary<string, string>>() { ErrorMessage = ex.Message };
        }
    }
}
