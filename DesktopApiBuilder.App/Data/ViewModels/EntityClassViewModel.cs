using DesktopApiBuilder.App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityClassViewModel : ICloneable
{
    [Required(ErrorMessage = "Field is required")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Cannot use special symbols")]
    [CustomValidation(typeof(ValidationHelper), "ValidateForKeyword")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "Field is required")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Cannot use special symbols")]
    [CustomValidation(typeof(ValidationHelper), "ValidateForKeyword")]
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
