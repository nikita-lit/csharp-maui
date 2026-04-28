using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Example;

public static class LanguageService
{
    private static ResourceManager _resourceManager = new ResourceManager(
            "Example.Resources.Localization.AppResources",
            Assembly.GetExecutingAssembly());

    public static event Action? LanguageChanged;

    public static void ChangeLanguage(string languageCode)
    {
        var culture = new CultureInfo(languageCode);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        LanguageChanged?.Invoke();
    }

    public static string Get(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentCulture) ?? key;
    }
}