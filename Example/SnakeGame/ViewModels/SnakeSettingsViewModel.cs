using Example.SnakeGame.Models;

namespace Example.SnakeGame.ViewModels;

public class SnakeSettingsViewModel : BaseViewModel
{
    public string PlayerName
    {
        get;
        set { field = value; OnPropertyChanged(); }
    } = string.Empty;

    public string SelectedTheme
    {
        get;
        set { field = value; OnPropertyChanged(); }
    } = string.Empty;

    public int SelectedSpeed
    {
        get;
        set { field = value; OnPropertyChanged(); }
    }

    public SnakeSettingsViewModel()
    {
        Load();
    }

    public void Load()
    {
        PlayerName = Preferences.Get("snake_player_name", LanguageService.Get("SnakeDefaultPlayer"));
        SelectedTheme = Theme.LoadSelected().Name;
        SelectedSpeed = Preferences.Get("snake_speed", 200);
    }

    public void Save()
    {
        var name = string.IsNullOrWhiteSpace(PlayerName)
            ? LanguageService.Get("SnakeDefaultPlayer")
            : PlayerName.Trim();

        PlayerName = name;
        Preferences.Set("snake_player_name", name);
        Preferences.Set("snake_speed", SelectedSpeed);
        Theme.ChangeTheme(SelectedTheme);
    }
}
