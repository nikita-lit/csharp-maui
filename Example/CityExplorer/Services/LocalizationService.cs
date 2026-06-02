using System.Globalization;
using System.Resources;

namespace Example.CityExplorer.Services;

public class LocalizationService
{
    private readonly ResourceManager _resourceManager = new("Example.Resources.Localization.CityExplorer.AppResources", typeof(LocalizationService).Assembly);
    private CultureInfo _currentCulture = new("et");

    public event EventHandler? LanguageChanged;

    public string CurrentLanguage => _currentCulture.TwoLetterISOLanguageName;

    public string this[string key] => GetString(key);

    private string GetString(string key) =>
        _resourceManager.GetString(key, _currentCulture) ?? key;

    public void SetLanguage(string languageName)
    {
        var cultureName = languageName switch
        {
            "English" => "en",
            "Русский" => "ru",
            _ => "et"
        };

        _currentCulture = new CultureInfo(cultureName);
        CultureInfo.DefaultThreadCurrentCulture = _currentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = _currentCulture;
        LanguageChanged?.Invoke(this, EventArgs.Empty);
    }
}
