namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityClassViewModel
{
    public string Name { get; set; }
    public string PluralName { get; set; }
    public List<EntityPropViewModel> Properties { get; set; }
}
