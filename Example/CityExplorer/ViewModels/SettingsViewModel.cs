using System.Collections.ObjectModel;
using System.Windows.Input;
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
        ChangeLanguageCommand = new Command<string>(ChangeLanguage);
        _localizationService.LanguageChanged += (_, _) => RefreshLocalizedText();
    }

    public ObservableCollection<string> Languages { get; }
    public ICommand ChangeLanguageCommand { get; }

    public string SettingsTitle => _localizationService["SettingsTitle"];
    public string PageHeading => _localizationService["SettingsHeading"];
    public string LanguageLabel => _localizationService["LanguageLabel"];

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (SetProperty(ref _currentLanguage, value))
                ChangeLanguage(value);
        }
    }

    private void ChangeLanguage(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return;

        _localizationService.SetLanguage(language);
    }

    private void RefreshLocalizedText()
    {
        OnPropertyChanged(nameof(SettingsTitle));
        OnPropertyChanged(nameof(PageHeading));
        OnPropertyChanged(nameof(LanguageLabel));
    }
}
