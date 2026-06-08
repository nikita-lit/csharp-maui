using System.Globalization;
using System.Resources;

namespace Example.CityExplorer.Services;

public static class LocalizationService
{
    private static readonly ResourceManager ResourceManager =
        new("Example.Resources.Localization.CityExplorer.AppResources", typeof(LocalizationService).Assembly);

    private static CultureInfo _currentCulture = new("et");

    public static string CurrentLanguage => _currentCulture.TwoLetterISOLanguageName;

    public static event EventHandler? LanguageChanged;

    public static string Get( string key )
    {
        return ResourceManager.GetString( key, _currentCulture ) ?? key;
    }

    public static void SetLanguage( string languageName )
    {
        var cultureName = languageName switch
        {
            "English" => "en",
            "Русский" => "ru",
            _ => "et"
        };

        _currentCulture = new CultureInfo( cultureName );
        CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;
        LanguageChanged?.Invoke( null, EventArgs.Empty );
    }
}