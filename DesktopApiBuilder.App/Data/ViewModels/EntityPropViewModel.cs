namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityPropViewModel : ICloneable
{
    public string Name { get; set; }
    public string Type { get; set; }

    public object Clone() => MemberwiseClone();
}
