namespace Example.SnakeGame;

public partial class SnakeGameSettings : ContentPage
{
    private Theme _theme;
    private string _selectedTheme;
    private int _selectedSpeed;

    public SnakeGameSettings()
    {
        InitializeComponent();

        _theme = Theme.LoadSelected();
        _selectedTheme = _theme.Name;
        _selectedSpeed = Preferences.Get("snake_speed", 200);

        PlayerNameEntry.Text = Preferences.Get("snake_player_name", "Mängija");

        ApplyTheme();
        HighlightSelectedTheme();
        HighlightSelectedSpeed();
    }

    private void ApplyTheme()
    {
        _theme.Apply(this);

        SettingsTitleLabel.TextColor = _theme.TextColor;
        NameLabel.TextColor = _theme.TextColor;
        ThemeLabel.TextColor = _theme.TextColor;
        SpeedLabel.TextColor = _theme.TextColor;

        PlayerNameEntry.TextColor = _theme.TextColor;
        PlayerNameEntry.PlaceholderColor = _theme.GridColor;
        PlayerNameEntry.BackgroundColor = _theme.GridColor;

        SaveButton.BackgroundColor = _theme.ButtonColor;
        SaveButton.TextColor = _theme.ButtonTextColor;

        foreach (var btn in new[] { LightBtn, DarkBtn, ColorfulBtn, SlowBtn, NormalBtn, FastBtn })
            StyleButton(btn, false);
    }

    private void HighlightSelectedTheme()
    {
        StyleButton(LightBtn, _selectedTheme == "Hele");
        StyleButton(DarkBtn, _selectedTheme == "Tume");
        StyleButton(ColorfulBtn, _selectedTheme == "Värviline");
    }

    private void HighlightSelectedSpeed()
    {
        StyleButton(SlowBtn, _selectedSpeed == 300);
        StyleButton(NormalBtn, _selectedSpeed == 200);
        StyleButton(FastBtn, _selectedSpeed == 120);
    }

    private void StyleButton(Button button, bool selected)
    {
        button.BackgroundColor = selected ? _theme.ButtonColor : _theme.GridColor;
        button.TextColor = selected ? _theme.ButtonTextColor : _theme.TextColor;
    }

    private void OnThemeSelected(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            _selectedTheme = btn.Text;
            _theme = Theme.GetByName(_selectedTheme);
            ApplyTheme();
            HighlightSelectedTheme();
            HighlightSelectedSpeed();
        }
    }

    private void OnSpeedSelected(object? sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            _selectedSpeed = btn.Text switch
            {
                "Aeglane" => 300,
                "Normaalne" => 200,
                "Kiire" => 120,
                _ => 200
            };
            HighlightSelectedSpeed();
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        string name = string.IsNullOrWhiteSpace(PlayerNameEntry.Text) ? "Mängija" : PlayerNameEntry.Text.Trim();
        Preferences.Set("snake_player_name", name);
        Theme.SaveSelected(_selectedTheme);
        Preferences.Set("snake_speed", _selectedSpeed);

        await DisplayAlertAsync("Salvestatud", "Seaded on salvestatud!", "OK");
        await Navigation.PopAsync();
    }
}
