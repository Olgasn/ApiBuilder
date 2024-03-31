namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityClassViewModel : ICloneable
{
    public string? Name { get; set; }
    public string? PluralName { get; set; }
    public List<EntityPropViewModel> Properties { get; set; }

    public object Clone()
    {
        return new EntityClassViewModel
        {
            Name = Name,
            PluralName = PluralName,
            Properties = Properties.Select(p => (EntityPropViewModel)p.Clone()).ToList(),
        };
    }
}
