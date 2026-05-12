namespace Example.SnakeGame;

public class SnakeSettingsViewModel : BaseViewModel
{
    public string PlayerName
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public string SelectedTheme
    {
        get;
        set => SetProperty(ref field, value);
    } = string.Empty;

    public int SelectedSpeed
    {
        get;
        set => SetProperty(ref field, value);
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
        ThemeService.ChangeTheme(SelectedTheme);
    }
}
