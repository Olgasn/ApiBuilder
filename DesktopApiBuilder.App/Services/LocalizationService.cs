using System.Globalization;

namespace DesktopApiBuilder.App.Services;

public static class LocalizationService
{
    private readonly static CultureInfo _defaultCulture = new("en-US");
    private static CultureInfo? _currentCulture;

    public static CultureInfo GetCurrentCulture() => _currentCulture ?? _defaultCulture;

    public static void SetCurrentCulture(string culture) => _currentCulture = new CultureInfo(culture);
}
