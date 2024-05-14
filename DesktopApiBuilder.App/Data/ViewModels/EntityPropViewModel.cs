using DesktopApiBuilder.App.Helpers;
using System.ComponentModel.DataAnnotations;

namespace DesktopApiBuilder.App.Data.ViewModels;

public class EntityPropViewModel : ICloneable
{
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    [CustomValidation(typeof(ValidationHelper), "ValidateForKeyword")]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    public object Clone() => MemberwiseClone();
}
