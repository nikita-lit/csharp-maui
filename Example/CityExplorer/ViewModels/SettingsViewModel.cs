using System.Collections.ObjectModel;
using Example.CityExplorer.Services;

namespace Example.CityExplorer.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private string _currentLanguage = "Eesti";

    public SettingsViewModel()
    {
        Languages = ["Eesti", "English", "Русский"];
        LocalizationService.LanguageChanged += ( _, _ ) => RefreshLocalizedText();
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
            if ( _currentLanguage == value )
                return;

            _currentLanguage = value;
            OnPropertyChanged();
            ChangeLanguage( value );
        }
    }

    public void Load()
    {
        SettingsTitle = LocalizationService.Get( "SettingsTitle" );
        PageHeading = LocalizationService.Get( "SettingsHeading" );
        LanguageLabel = LocalizationService.Get( "LanguageLabel" );
        _currentLanguage = LocalizationService.CurrentLanguage switch
        {
            "en" => "English",
            "ru" => "Русский",
            _ => "Eesti"
        };

        OnPropertyChanged( nameof(SettingsTitle) );
        OnPropertyChanged( nameof(PageHeading) );
        OnPropertyChanged( nameof(LanguageLabel) );
        OnPropertyChanged( nameof(CurrentLanguage) );
    }

    private static void ChangeLanguage( string language )
    {
        if ( string.IsNullOrWhiteSpace( language ) )
            return;

        LocalizationService.SetLanguage( language );
    }

    private void RefreshLocalizedText()
    {
        Load();
    }
}