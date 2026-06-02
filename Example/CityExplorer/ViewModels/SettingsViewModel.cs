using System.Collections.ObjectModel;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly LocalizationService _localizationService;
    private string _currentLanguage = "Eesti";

    public SettingsViewModel(LocalizationService localizationService)
    {
        _localizationService = localizationService;
        Languages = ["Eesti", "English", "Русский"];
        _localizationService.LanguageChanged += (_, _) => RefreshLocalizedText();
        Load();
    }

    public ObservableCollection<string> Languages { get; }
    public string SettingsTitle { get; private set; } = string.Empty;
    public string PageHeading { get; private set; } = string.Empty;
    public string LanguageLabel { get; private set; } = string.Empty;

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage == value)
                return;

            _currentLanguage = value;
            OnPropertyChanged();
            ChangeLanguage(value);
        }
    }

    public void Load()
    {
        SettingsTitle = _localizationService["SettingsTitle"];
        PageHeading = _localizationService["SettingsHeading"];
        LanguageLabel = _localizationService["LanguageLabel"];
        _currentLanguage = _localizationService.CurrentLanguage switch
        {
            "en" => "English",
            "ru" => "Русский",
            _ => "Eesti"
        };

        OnPropertyChanged(nameof(SettingsTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(LanguageLabel));
        OnPropertyChanged(nameof(CurrentLanguage));
    }

    private void ChangeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return;

        _localizationService.SetLanguage(language);
    }

    private void RefreshLocalizedText()
    {
        Load();
    }
}
