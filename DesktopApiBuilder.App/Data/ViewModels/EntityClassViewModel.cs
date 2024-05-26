using DesktopApiBuilder.App.Helpers;
using DesktopApiBuilder.App.Resources.Languages;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;


namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityClassViewModel : ICloneable
{
    [Required(ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "FieldIsRequiredErrorAttribute")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "SpecialSymbolsErrorAttribute")]
    [CustomValidation(typeof(ValidationHelper), "ValidateForKeyword", ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "CsharpErrorAttribute")]
    public string? Name { get; set; }

    [Required(ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "FieldIsRequiredErrorAttribute")]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "SpecialSymbolsErrorAttribute")]
    [CustomValidation(typeof(ValidationHelper), "ValidateForKeyword", ErrorMessageResourceType = typeof(Localization), ErrorMessageResourceName = "CsharpErrorAttribute")]
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
